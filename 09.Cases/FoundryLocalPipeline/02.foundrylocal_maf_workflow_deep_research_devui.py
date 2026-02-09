"""
Deep Research Workflow using Microsoft Agent Framework Workflows with DevUI

This module implements a deep research system using MAF workflow patterns,
with a tool-enabled agent approach and DevUI visualization.

Key Features:
- Research agent with integrated search_web tool
- Simplified workflow with fewer executors
- Iterative research loop with continuation logic
- Final report generation
- DevUI web interface for interactive workflow execution

Workflow Structure:
1. ResearchAgentExecutor → research_agent (with search_web tool) → IterationControl
2. IterationControl → (if CONTINUE) → back to ResearchAgentExecutor
3. IterationControl → (if COMPLETE) → FinalReportExecutor → final_reporter_agent → Output

Based on:
- https://github.com/microsoft/agent-framework/tree/main/python/samples/getting_started/workflows
- https://github.com/microsoft/agent-framework/tree/main/python/samples/getting_started/devui/workflow_agents
"""

import asyncio
import logging
from enum import Enum
from typing import Annotated, Dict, Any, List
from datetime import datetime
from dotenv import load_dotenv
from agent_framework import (
    Executor,
    WorkflowBuilder,
    WorkflowContext,
    handler,
    AgentExecutorRequest,
    AgentExecutorResponse,
    ChatMessage,
    ChatAgent,
    WorkflowViz
)
from agent_framework.devui import serve
from agent_framework_foundry_local import FoundryLocalClient
from utils import web_search

# Load environment variables
load_dotenv()

# Setup logging
logging.basicConfig(level=logging.INFO, format="%(message)s")
logger = logging.getLogger(__name__)


# ============================================================================
# Tool Function for Agent
# ============================================================================

def search_web(
    query: Annotated[str, "The search query to execute on Google"],
    max_results: Annotated[int, "Maximum number of results (default: 3)"] = 3,
    fetch_full_page: Annotated[bool, "Whether to fetch full page content (default: True)"] = True,
    engines: Annotated[str, "Search engine to use (default: 'google')"] = "google"
) -> str:
    """
    Search the web using SerpAPI with Google search engine.
    Returns formatted search results with titles, URLs, and content snippets.
    Default fetches full page content for deep research.
    """
    try:
        # Parse engines parameter
        engine_list = [e.strip() for e in engines.split(",")]
        
        # Execute search
        results = web_search(
            query=query,
            max_results=max_results,
            fetch_full_page=fetch_full_page,
            engines=engine_list
        )
        
        # Format results
        if not results:
            return "No search results found."
        
        formatted_output = f"Search Results for '{query}':\n\n"
        for i, result in enumerate(results, 1):
            formatted_output += f"### Result {i}\n"
            formatted_output += f"**Title:** {result['title']}\n"
            formatted_output += f"**URL:** {result['url']}\n"
            formatted_output += f"**Snippet:** {result['content']}\n"
            if fetch_full_page and result.get('raw_content'):
                # Truncate raw content for display
                raw_content = result['raw_content']
                if raw_content and len(raw_content) > 1000:
                    raw_content = raw_content[:1000] + "... [truncated]"
                formatted_output += f"**Full Content Preview:** {raw_content}\n"
            formatted_output += "\n---\n\n"
        
        return formatted_output
        
    except Exception as e:
        return f"Error during web search: {str(e)}"


# ============================================================================
# Enums for Control Flow
# ============================================================================

class ResearchSignal(Enum):
    """Signals to control the research workflow iteration"""
    INIT = "init"
    CONTINUE = "continue"
    COMPLETE = "complete"


# ============================================================================
# Data Models (Simplified)
# ============================================================================

class ResearchState:
    """State object to track research progress"""
    def __init__(self, topic: str, max_iterations: int):
        self.topic = topic
        self.max_iterations = max_iterations
        self.current_iteration = 0
        self.summaries: List[str] = []
    
    def increment_iteration(self):
        self.current_iteration += 1
    
    def should_continue(self) -> bool:
        return self.current_iteration < self.max_iterations
    
    def add_summary(self, summary: str):
        self.summaries.append(summary)
    
    def get_all_summaries(self) -> str:
        return "\n\n".join([f"## Iteration {i+1}\n{s}" for i, s in enumerate(self.summaries)])


class IterationDecision:
    """Decision about whether to continue research"""
    def __init__(self, signal: ResearchSignal, state: ResearchState, 
                 latest_summary: str | None = None):
        self.signal = signal
        self.state = state
        self.latest_summary = latest_summary
    
    def __str__(self):
        return f"Decision: {self.signal.value} (iter {self.state.current_iteration}/{self.state.max_iterations})"


# ============================================================================
# Workflow Executors (Simplified)
# ============================================================================

class StartExecutor(Executor):
    """Start executor that accepts user input and creates initial IterationDecision"""
    
    def __init__(self, state: ResearchState, id: str = "start_executor"):
        super().__init__(id=id)
        self.state = state
    
    @handler
    async def start_workflow(
        self,
        user_input: str | dict,
        ctx: WorkflowContext[IterationDecision]
    ) -> None:
        """Accept user input (research topic or dict) and start the workflow"""
        
        # Handle different input types
        if isinstance(user_input, dict):
            # If dict, look for 'topic' or 'message' key
            topic = user_input.get('topic') or user_input.get('message') or user_input.get('text') or str(user_input)
        else:
            # If string, use directly as topic
            topic = str(user_input)
        
        # Update state with the new topic if provided
        if topic and topic.strip():
            self.state.topic = topic.strip()
        
        logger.info(f"\n🚀 Starting Deep Research on: {self.state.topic}")
        logger.info(f"📊 Max Iterations: {self.state.max_iterations}\n")
        
        # Create initial decision
        initial_decision = IterationDecision(
            signal=ResearchSignal.INIT,
            state=self.state
        )
        
        # Send to next executor
        await ctx.send_message(initial_decision)


class ResearchAgentExecutor(Executor):
    """Main research executor that uses an agent with search_web tool"""
    
    def __init__(self, id: str = "research_agent_executor"):
        super().__init__(id=id)
    
    @handler
    async def conduct_research(
        self, 
        decision: IterationDecision, 
        ctx: WorkflowContext[AgentExecutorRequest]
    ) -> None:
        """Conduct research using the agent with tools"""
        
        state = decision.state
        
        if decision.signal == ResearchSignal.INIT:
            # Initial research
            prompt = f"""Research Topic: {state.topic}

Please conduct comprehensive research on this topic. Follow these steps:

1. Generate a specific search query to gather initial information
2. Use the search_web tool to search Google (it will automatically fetch full page content)
3. Analyze the results and provide a summary
4. Identify what additional information would be valuable for deeper understanding

Start your research now."""
            
            logger.info(f"\n🔍 Starting initial research on: {state.topic}")
        
        elif decision.signal == ResearchSignal.CONTINUE:
            # Follow-up research
            previous_summaries = state.get_all_summaries()
            
            prompt = f"""Research Topic: {state.topic}

Previous Research:
{previous_summaries}

Based on your previous research, please:

1. Identify a specific knowledge gap or area that needs deeper exploration
2. Generate a targeted follow-up search query
3. Use the search_web tool to gather additional information
4. Update your summary with the new findings
5. Explain how this new information addresses the knowledge gap

Continue your research now."""
            
            logger.info(f"\n🔄 Continuing research (iteration {state.current_iteration}/{state.max_iterations})")
        
        else:
            # Should not reach here
            return
        
        # Send request to the research agent
        await ctx.send_message(
            AgentExecutorRequest(
                messages=[ChatMessage("user", text=prompt)],
                should_respond=True
            ),
            target_id="research_agent"
        )


class IterationControlExecutor(Executor):
    """Controls the iteration loop and decides when to continue or complete"""
    
    def __init__(self, id: str = "iteration_control"):
        super().__init__(id=id)
    
    @handler
    async def control_iteration(
        self, 
        research_response: AgentExecutorResponse, 
        ctx: WorkflowContext[IterationDecision]
    ) -> None:
        """Decide whether to continue research or complete"""
        
        summary_text = research_response.agent_response.text
        state = research_response.agent_response.context.get("state") if hasattr(research_response.agent_response, "context") else None
        
        # If no state in response, we need to get it from somewhere
        # For now, we'll create a workaround by tracking it here
        if not hasattr(self, '_state'):
            # This shouldn't happen, but handle gracefully
            logger.warning("⚠️ Warning: State not found, creating default state")
            return
        
        state = self._state
        state.add_summary(summary_text)
        state.increment_iteration()
        
        logger.info(f"\n📝 Research Summary (Iteration {state.current_iteration}):")
        logger.info(f"{summary_text}\n")
        logger.info("=" * 80)
        
        # Decide next action
        if state.should_continue():
            # Continue research
            decision = IterationDecision(
                signal=ResearchSignal.CONTINUE,
                state=state,
                latest_summary=summary_text
            )
            logger.info(f"\n🔄 Continuing to iteration {state.current_iteration + 1}/{state.max_iterations}")
        else:
            # Complete the research
            decision = IterationDecision(
                signal=ResearchSignal.COMPLETE,
                state=state,
                latest_summary=summary_text
            )
            logger.info(f"\n✅ Research Complete! ({state.current_iteration} iterations)")
        
        await ctx.send_message(decision)
    
    def set_state(self, state: ResearchState):
        """Set the state for tracking"""
        self._state = state


class FinalReportExecutor(Executor):
    """Generates final research report"""
    
    def __init__(self, id: str = "final_report"):
        super().__init__(id=id)
    
    @handler
    async def generate_final_report(
        self, 
        decision: IterationDecision, 
        ctx: WorkflowContext[AgentExecutorRequest]
    ) -> None:
        """Generate comprehensive final report"""
        
        if decision.signal != ResearchSignal.COMPLETE:
            # Not ready for final report
            return
        
        state = decision.state
        all_summaries = state.get_all_summaries()
        
        prompt = f"""Based on all research conducted, provide a comprehensive final summary of: {state.topic}

All Research Iterations:
{all_summaries}

Your final report should:
1. Integrate all findings from the research iterations
2. Highlight key insights and important details
3. Include proper citations with URLs
4. Be well-structured and easy to read
5. Provide a coherent narrative that addresses the research topic

Provide your final research report now."""
        
        logger.info("\n📄 Generating Final Report...")
        
        await ctx.send_message(
            AgentExecutorRequest(
                messages=[ChatMessage("user", text=prompt)],
                should_respond=True
            ),
            target_id="final_reporter_agent"
        )


class OutputExecutor(Executor):
    """Outputs the final research report"""
    
    def __init__(self, id: str = "output_executor"):
        super().__init__(id=id)
    
    @handler
    async def output_report(
        self, 
        final_response: AgentExecutorResponse, 
        ctx: WorkflowContext[None, str]
    ) -> None:
        """Output the final research report"""
        
        final_report = final_response.agent_response.text
        
        logger.info("\n" + "=" * 80)
        logger.info("📊 FINAL RESEARCH REPORT")
        logger.info("=" * 80)
        logger.info(f"\n{final_report}\n")
        logger.info("=" * 80)
        
        # Yield the final output
        await ctx.yield_output(final_report)


# ============================================================================
# Agent Creation Functions
# ============================================================================

def create_research_agent(model_id: str = "qwen2.5-1.5b-instruct-generic-cpu:4") -> ChatAgent:
    """Create the main research agent with search_web tool"""
    current_date = datetime.now().strftime("%B %d, %Y")
    
    client = FoundryLocalClient(model_id=model_id)
    return client.as_agent(
        name="research_agent",
        instructions=(
            f"You are an expert research assistant conducting deep web research. Current date: {current_date}\n\n"
            "Your research workflow:\n"
            "1. **Generate Search Query**: Create a targeted web search query based on the research topic\n"
            "2. **Execute Search**: Use the search_web tool to find relevant information\n"
            "3. **Analyze Results**: Carefully review and summarize the search results\n"
            "4. **Identify Knowledge Gaps**: Reflect on what information is missing or needs clarification\n"
            "5. **Iterate**: Generate follow-up queries to address knowledge gaps\n\n"
            "CRITICAL RULES:\n"
            "- Always use the search_web tool to gather information\n"
            "- Provide comprehensive summaries with proper citations (include URLs)\n"
            "- When summarizing, highlight the most relevant information\n"
            "- Identify specific knowledge gaps for follow-up research\n"
            "- Be thorough but concise in your analysis"
        ),
        tools=search_web,  # Tool integrated here!
        default_options={"temperature": 0.7, "max_tokens": 4096}
    )


def create_final_reporter_agent(model_id: str = "qwen2.5-1.5b-instruct-generic-cpu:4") -> ChatAgent:
    """Create the final report generation agent"""
    current_date = datetime.now().strftime("%B %d, %Y")
    
    client = FoundryLocalClient(model_id=model_id)
    return client.as_agent(
        name="final_reporter_agent",
        instructions=(
            f"You are an expert research report writer. Current date: {current_date}\n\n"
            "Your role is to synthesize multiple research iterations into a comprehensive, "
            "well-structured final report that:\n"
            "1. Integrates all findings cohesively\n"
            "2. Highlights key insights and discoveries\n"
            "3. Includes proper citations and sources\n"
            "4. Presents information in a clear, logical flow\n"
            "5. Provides actionable conclusions\n\n"
            "Create professional, publication-quality research reports."
        ),
        default_options={"temperature": 0.7, "max_tokens": 4096}
    )


# ============================================================================
# Workflow Builder (Simplified)
# ============================================================================

def build_research_workflow(
    research_topic: str = "Latest developments in AI and Machine Learning",
    max_iterations: int = 3,
    max_results: int = 3,
    fetch_full_page: bool = True,
    model_id: str = "qwen2.5-1.5b-instruct-generic-cpu:4"
):
    """
    Build the deep research workflow (simplified with tool-enabled agent)
    
    Args:
        research_topic: The research topic
        max_iterations: Maximum number of research iterations
        max_results: Maximum search results per query (for tool)
        fetch_full_page: Whether to fetch full page content (for tool)
        model_id: Model ID for the agents
    
    Returns:
        Configured workflow ready to run
    """
    
    # Create research state
    state = ResearchState(topic=research_topic, max_iterations=max_iterations)
    
    # Create the workflow with simplified structure:
    # 1. ResearchAgentExecutor → sends task to research agent
    # 2. research_agent → uses search_web tool to research
    # 3. IterationControl → decides continue or complete
    # 4a. If CONTINUE → loop back to ResearchAgentExecutor
    # 4b. If COMPLETE → FinalReportExecutor → final_reporter_agent → Output
    
    workflow_builder = WorkflowBuilder(
        name="Deep Research Workflow",
        description=f"Multi-agent deep research workflow with iterative web search (Topic: {research_topic})"
    )
    
    # Create executors with state reference
    start_executor = StartExecutor(state=state)
    iteration_control = IterationControlExecutor()
    iteration_control.set_state(state)
    
    # Register executors
    workflow_builder.register_executor(
        lambda: start_executor,
        name="start_executor"
    )
    workflow_builder.register_executor(
        lambda: ResearchAgentExecutor(),
        name="research_executor"
    )
    workflow_builder.register_executor(
        lambda: iteration_control,
        name="iteration_control"
    )
    workflow_builder.register_executor(
        lambda: FinalReportExecutor(),
        name="final_report"
    )
    workflow_builder.register_executor(
        lambda: OutputExecutor(),
        name="output_executor"
    )
    
    # Register agents
    workflow_builder.register_agent(
        lambda: create_research_agent(model_id),
        name="research_agent"
    )
    workflow_builder.register_agent(
        lambda: create_final_reporter_agent(model_id),
        name="final_reporter_agent"
    )
    
    # Define edges for the research loop
    workflow_builder.add_edge("start_executor", "research_executor")
    workflow_builder.add_edge("research_executor", "research_agent")
    workflow_builder.add_edge("research_agent", "iteration_control")
    
    # Conditional edges from iteration_control
    workflow_builder.add_edge(
        "iteration_control", 
        "research_executor",
        condition=lambda decision: decision.signal == ResearchSignal.CONTINUE
    )
    workflow_builder.add_edge(
        "iteration_control",
        "final_report",
        condition=lambda decision: decision.signal == ResearchSignal.COMPLETE
    )
    
    # Final report generation
    workflow_builder.add_edge("final_report", "final_reporter_agent")
    workflow_builder.add_edge("final_reporter_agent", "output_executor")
    
    # Set start executor and build
    workflow = workflow_builder.set_start_executor("start_executor").build()
    
    return workflow, state


# ============================================================================
# Main Execution with DevUI
# ============================================================================

def main():
    """Launch the Deep Research Workflow in DevUI"""
    
    logger.info("=" * 80)
    logger.info("🔬 DEEP RESEARCH WORKFLOW (MAF Workflows + DevUI)")
    logger.info("=" * 80)
    logger.info("\nThis workflow demonstrates:")
    logger.info("- Research agent with integrated search_web tool")
    logger.info("- Iterative research loop with Google search")
    logger.info("- Automatic continuation based on max iterations")
    logger.info("- Final report synthesis from all research iterations")
    logger.info("\nWorkflow Path:")
    logger.info("  ResearchExecutor → ResearchAgent (with search_web tool)")
    logger.info("  → IterationControl → [CONTINUE] → (loop back)")
    logger.info("  → [COMPLETE] → FinalReport → FinalReporter → Output")
    logger.info("\n" + "=" * 80)
    
    # Build the workflow with default parameters
    # Note: The initial topic is set to a default, but can be changed in DevUI
    workflow, state = build_research_workflow(
        research_topic="Latest developments in Large Language Models in 2025",
        max_iterations=3,
        max_results=3,
        fetch_full_page=True,
        model_id="qwen2.5-1.5b-instruct-generic-cpu:4"
    )

    print("Generating workflow visualization...")
    viz = WorkflowViz(workflow)
    # Print out the mermaid string.
    print("Mermaid string: \n=======")
    print(viz.to_mermaid())
    print("=======")
    # Print out the DiGraph string.
    print("DiGraph string: \n=======")
    print(viz.to_digraph())
    print("=======")
    svg_file = viz.export(format="svg")
    print(f"SVG file saved to: {svg_file}")
    
    logger.info("\n🚀 Starting DevUI Server...")
    logger.info("Available at: http://localhost:8093")
    logger.info("\nHow to use:")
    logger.info("1. Enter your research topic as text in the input field")
    logger.info("2. Click 'Run' to start the research workflow")
    logger.info("3. Watch as the workflow iteratively researches your topic")
    logger.info("\nExample inputs:")
    logger.info("  - 'Latest trends in renewable energy'")
    logger.info("  - 'Advances in quantum computing in 2025'")
    logger.info("  - 'Impact of AI on healthcare'")
    logger.info("\n" + "=" * 80 + "\n")
    
    # Launch DevUI
    serve(entities=[workflow], port=8093, auto_open=True,instrumentation_enabled=True)


# ============================================================================
# CLI Execution (without DevUI)
# ============================================================================

async def run_cli():
    """Run the workflow from command line (without DevUI)"""
    
    # Configuration
    research_topic = "Latest developments in Large Language Models in 2025"
    max_iterations = 3
    max_results = 3
    fetch_full_page = True
    model_id = "qwen2.5-1.5b-instruct-generic-cpu:4"
    
    logger.info("=" * 80)
    logger.info("🔬 DEEP RESEARCH WORKFLOW (CLI Mode)")
    logger.info("=" * 80)
    logger.info(f"\n📌 Research Topic: {research_topic}")
    logger.info(f"🔢 Max Iterations: {max_iterations}")
    logger.info(f"📊 Max Results per Query: {max_results}")
    logger.info(f"📄 Fetch Full Pages: {fetch_full_page}")
    logger.info(f"🤖 Model: {model_id}")
    logger.info("\n" + "=" * 80)
    
    # Build the workflow
    workflow, state = build_research_workflow(
        research_topic=research_topic,
        max_iterations=max_iterations,
        max_results=max_results,
        fetch_full_page=fetch_full_page,
        model_id=model_id
    )
    
    # Run the workflow
    logger.info("\n🚀 Starting workflow...\n")
    
    # Initialize with INIT signal
    initial_decision = IterationDecision(
        signal=ResearchSignal.INIT,
        state=state
    )
    
    # Run the workflow (non-streaming mode for simplicity)
    events = await workflow.run(initial_decision)
    
    # Get outputs
    outputs = events.get_outputs()
    if outputs:
        logger.info(f"\n✨ Workflow completed with {len(outputs)} outputs")
        for output in outputs:
            logger.info(f"\n{output}")
    
    logger.info(f"\nFinal state: {events.get_final_state()}")
    
    logger.info("\n" + "=" * 80)
    logger.info("🎉 WORKFLOW COMPLETE!")
    logger.info("=" * 80)


if __name__ == "__main__":
    import sys
    
    if len(sys.argv) > 1 and sys.argv[1] == "--cli":
        # Run in CLI mode
        asyncio.run(run_cli())
    else:
        # Run with DevUI (default)
        main()
