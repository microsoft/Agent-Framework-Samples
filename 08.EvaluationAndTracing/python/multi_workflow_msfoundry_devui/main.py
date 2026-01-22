"""Main entry point for Content Workflow with Azure AI Foundry"""

import asyncio
import logging
import os
from dotenv import load_dotenv

# Load environment variables first
load_dotenv()

from workflow.workflow import create_workflow
from agent_framework.devui import serve


def main():
    """Create workflow and launch DevUI"""
    # Setup logging
    logging.basicConfig(level=logging.INFO, format="%(message)s")
    logger = logging.getLogger(__name__)

    logger.info("🚀 Starting Content Workflow with Conditional Logic")
    logger.info("📍 Available at: http://localhost:8090")
    logger.info("🔖 Entity ID: workflow_content")
    logger.info("📝 Workflow: Evangelist → Reviewer → (Conditional) → Publisher")
    logger.info("⚙️  Make sure 'az login' has been run for authentication")

    # Create workflow using AzureAIProjectAgentProvider pattern
    workflow = asyncio.run(create_workflow())
    
    # Launch server with the workflow (serve manages its own event loop)
    serve(entities=[workflow], port=8090, auto_open=True)


if __name__ == "__main__":
    main()
