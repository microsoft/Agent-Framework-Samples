
import os
import logging

from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint
from workflow import workflow  # 🏗️ The AGUI workflow
from fastapi import FastAPI

# Enable debug logging
logging.basicConfig(level=logging.DEBUG, format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

app = FastAPI(title="Travel Agent AG-UI Server")

logger.info("Creating agent from workflow...")
agent = workflow.as_agent(name="Travel Agent")
logger.info(f"Agent created: {agent}")

# Register the AG-UI endpoint
logger.info("Registering AG-UI endpoint...")
add_agent_framework_fastapi_endpoint(app, agent, "/")
logger.info("AG-UI endpoint registered")

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=8888)