from agent_framework import WorkflowBuilder  # 🏗️ Workflow orchestration tools
from frontdesk_agent import front_desk_agent  # 🧑‍💼 Front Desk Travel Agent
from reviewer_agent import reviewer_agent  # 🧑‍💼 Reviewer Agent


workflow = WorkflowBuilder().set_start_executor(front_desk_agent).add_edge(front_desk_agent, reviewer_agent).build()