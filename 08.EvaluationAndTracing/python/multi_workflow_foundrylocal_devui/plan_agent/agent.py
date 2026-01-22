"""Planning agent for the FoundryLocal workflow.

Parallels the evangelist/content generation agent in the GitHub Models
example but is focused purely on generating a structured plan that a
research agent can expand.
"""

import os
from dotenv import load_dotenv

load_dotenv()

try:
	from agent_framework.openai import OpenAIChatClient  # type: ignore
except ImportError:  # pragma: no cover
	raise SystemExit("agent_framework package not found. Install project dependencies first.")

PLAN_AGENT_NAME = "Plan-Agent"
PLAN_AGENT_INSTRUCTIONS = """
You are my planner, working with me to create 1 sample based on the researcher's findings.
"""

def _build_client() -> OpenAIChatClient:
	base_url = os.environ.get("FOUNDRYLOCAL_ENDPOINT")
	model_id = os.environ.get("FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME") 
	api_key = "nokey"
	if not base_url:
		raise RuntimeError("No model endpoint configured. Set FOUNDRYLOCAL_ENDPOINT or GITHUB_ENDPOINT.")
	return OpenAIChatClient(base_url=base_url, api_key=api_key, model_id=model_id)

try:
	_client = _build_client()
	plan_agent = _client.as_agent(
		instructions=PLAN_AGENT_INSTRUCTIONS,
		name=PLAN_AGENT_NAME,
	)
except Exception as e:  # pragma: no cover
	print(f"[plan_agent] initialization warning: {e}")
	plan_agent = None  # type: ignore

