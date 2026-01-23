"""Test with single agent instead of workflow."""

from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint
from frontend_agent import frontend_agent
from fastapi import FastAPI

app = FastAPI(title="Single Agent Test")

# Use single agent directly
add_agent_framework_fastapi_endpoint(app, frontend_agent, "/")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8890)
