# from dotenv import load_dotenv
from workflow import workflow  # 🏗️ The content workflow



# Load environment variables first, before importing agents
# load_dotenv()

def main():
    """Launch the content workflow in DevUI."""
    import logging
    from agent_framework.devui import serve

    # Setup logging
    logging.basicConfig(level=logging.INFO, format="%(message)s")
    logger = logging.getLogger(__name__)

    logger.info("Starting Content Workflow")
    logger.info("Available at: http://localhost:8090")
    logger.info("Entity ID: workflow_content")

    # Launch server with the workflow
    serve(entities=[workflow], port=8090, auto_open=True, tracing_enabled=True)

    


if __name__ == "__main__":
    main()
