from dataclasses import dataclass
from agent_framework import (
    AgentExecutor,
    AgentExecutorRequest,
    AgentExecutorResponse,
    ChatMessage,
    Role,
    WorkflowBuilder,
    WorkflowContext,
    executor,
)

from frontend_agent import frontend_agent
from concierge_agent import concierge_agent
# Create agent executors
frontend_executor = AgentExecutor(frontend_agent, id="frontend_agent")
concierge_executor = AgentExecutor(concierge_agent, id="concierge_agent")

# Build the conditional workflow
workflow = (
    WorkflowBuilder()
    .add_edge(frontend_executor, concierge_executor)
    .set_start_executor(frontend_executor)
    .build()
)
