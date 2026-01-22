"""Conditional Workflow for Content Review with Azure AI Foundry Agents"""

import os
from dataclasses import dataclass

from azure.identity.aio import AzureCliCredential
from dotenv import load_dotenv

from agent_framework import (
    AgentExecutor,
    AgentExecutorRequest,
    AgentExecutorResponse,
    ChatMessage,
    HostedCodeInterpreterTool,
    Role,
    WorkflowBuilder,
    WorkflowContext,
    executor,
)
from agent_framework.azure import AzureAIProjectAgentProvider

from evangelist_agent import EvangelistAgent, EVANGELIST_NAME, EVANGELIST_INSTRUCTIONS
from contentreview_agent import ReviewAgent, REVIEWER_NAME, REVIEWER_INSTRUCTIONS
from publisher_agent import PublisherAgent, PUBLISHER_NAME, PUBLISHER_INSTRUCTIONS

# Load environment variables
load_dotenv()


@dataclass
class ReviewResult:
    """Data class to hold review results"""
    review_result: str
    reason: str
    draft_content: str


@executor(id="to_evangelist_content_result")
async def to_evangelist_content_result(
    response: AgentExecutorResponse, 
    ctx: WorkflowContext[AgentExecutorRequest]
) -> None:
    """Convert evangelist agent response to structured format and forward to reviewer"""
    response_text = response.agent_response.text.strip()
    print(f"📝 [Workflow] Raw response from evangelist agent: {response_text}")
    
    parsed = EvangelistAgent.model_validate_json(response_text)
    user_msg = ChatMessage(Role.USER, text=parsed.draft_content)
    await ctx.send_message(AgentExecutorRequest(messages=[user_msg], should_respond=True))


@executor(id="to_reviewer_result")
async def to_reviewer_result(
    response: AgentExecutorResponse, 
    ctx: WorkflowContext[ReviewResult]
) -> None:
    """Convert reviewer agent response to structured format"""
    response_text = response.agent_response.text.strip()
    print(f"🔍 [Workflow] Raw response from reviewer agent: {response_text}")
    
    parsed = ReviewAgent.model_validate_json(response_text)
    await ctx.send_message(
        ReviewResult(
            review_result=parsed.review_result,
            reason=parsed.reason,
            draft_content=parsed.draft_content,
        )
    )


def select_targets(review: ReviewResult, target_ids: list[str]) -> list[str]:
    """
    Select workflow path based on review result
    
    Args:
        review: The review result containing decision
        target_ids: List of [handle_review_id, save_draft_id]
    
    Returns:
        List containing the selected target executor ID
    """
    handle_review_id, save_draft_id = target_ids
    if review.review_result == "Yes":
        print(f"✅ [Workflow] Review passed - routing to save_draft")
        return [save_draft_id]
    else:
        print(f"❌ [Workflow] Review failed - routing to handle_review")
        return [handle_review_id]


@executor(id="handle_review")
async def handle_review(review: ReviewResult, ctx: WorkflowContext[str]) -> None:
    """Handle review failures"""
    if review.review_result == "No":
        message = f"Review failed: {review.reason}, please revise the draft."
        print(f"⚠️ [Workflow] {message}")
        await ctx.yield_output(message)
    else:
        await ctx.send_message(
            AgentExecutorRequest(
                messages=[ChatMessage(Role.USER, text=review.draft_content)], 
                should_respond=True
            )
        )


@executor(id="save_draft")
async def save_draft(review: ReviewResult, ctx: WorkflowContext[AgentExecutorRequest]) -> None:
    """Save draft content by sending to publisher agent"""
    # Only called for approved drafts by selection_func
    await ctx.send_message(
        AgentExecutorRequest(
            messages=[ChatMessage(Role.USER, text=review.draft_content)], 
            should_respond=True
        )
    )


# Workflow creation function (matches notebook pattern)
async def create_workflow():
    """Create the conditional workflow with Azure AI Foundry agents"""
    async with (
        AzureCliCredential() as credential,
        AzureAIProjectAgentProvider(
            project_endpoint=os.environ["AZURE_AI_PROJECT_ENDPOINT"], 
            credential=credential
        ) as client,
    ):
        # Create evangelist agent with Bing Search
        evangelist_agent_obj = await client.create_agent(
            name=EVANGELIST_NAME,
            instructions=EVANGELIST_INSTRUCTIONS,
            model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
            tools=[
                {
                    "type": "bing_grounding",
                    "bing_grounding": {
                        "search_configurations": [
                            {
                                "project_connection_id": os.environ["BING_CONNECTION_ID"],
                            }
                        ]
                    },
                }
            ],
            # response_format=EvangelistAgent
        )
        evangelist_executor = AgentExecutor(evangelist_agent_obj, id="evangelist_agent")
        
        # Create reviewer agent
        reviewer_agent_obj = await client.create_agent(
            name=REVIEWER_NAME,
            instructions=REVIEWER_INSTRUCTIONS,
            model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
            # response_format=ReviewAgent
        )
        reviewer_executor = AgentExecutor(reviewer_agent_obj, id="reviewer_agent")
        
        # Create publisher agent with Code Interpreter
        publisher_agent_obj = await client.create_agent(
            name=PUBLISHER_NAME,
            instructions=PUBLISHER_INSTRUCTIONS,
            model=os.environ["AZURE_AI_MODEL_DEPLOYMENT_NAME"],
            tools=HostedCodeInterpreterTool()
            # response_format=PublisherAgent
        )
        publisher_executor = AgentExecutor(publisher_agent_obj, id="publisher_agent")

        # Build the conditional workflow
        workflow = (
            WorkflowBuilder()
            .set_start_executor(evangelist_executor)
            .add_edge(evangelist_executor, to_evangelist_content_result)
            .add_edge(to_evangelist_content_result, reviewer_executor)
            .add_edge(reviewer_executor, to_reviewer_result)
            .add_multi_selection_edge_group(
                to_reviewer_result,
                [handle_review, save_draft],
                selection_func=select_targets,
            )
            .add_edge(save_draft, publisher_executor)
            .build()
        )
        
        return workflow

