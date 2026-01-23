import os 

from agent_framework.openai import OpenAIChatClient  
from dotenv import load_dotenv  # 📁 Secure configuration loading

load_dotenv()  # 📁 Load .env file for secure config


chat_client = OpenAIChatClient(
    base_url=os.environ.get("GITHUB_ENDPOINT"),    # 🌐 GitHub Models API endpoint
    api_key=os.environ.get("GITHUB_TOKEN"),        # 🔑 Authentication token
    model_id=os.environ.get("GITHUB_MODEL_ID")     # 🎯 Selected AI model
)



FRONTEND_AGENT_NAMES = "FrontDesk"
FRONTEND_AGENT_INSTRUCTIONS = """
            You are a Front Desk Travel Agent with ten years of experience and are known for brevity as you deal with many customers.
            The goal is to provide the best activities and locations for a traveler to visit.
            Only provide a single recommendation per response.
            You're laser focused on the goal at hand.
            Don't waste time with chit chat.
            Consider suggestions when refining an idea.
            """


frontend_agent = chat_client.as_agent(
    instructions=FRONTEND_AGENT_INSTRUCTIONS,
    name=FRONTEND_AGENT_NAMES,
)
