import os 

from agent_framework.openai import OpenAIChatClient  
from dotenv import load_dotenv  # 📁 Secure configuration loading

load_dotenv()  # 📁 Load .env file for secure config


chat_client = OpenAIChatClient(
    base_url=os.environ.get("GITHUB_ENDPOINT"),    # 🌐 GitHub Models API endpoint
    api_key=os.environ.get("GITHUB_TOKEN"),        # 🔑 Authentication token
    model_id=os.environ.get("GITHUB_MODEL_ID")     # 🎯 Selected AI model
)


CONCIERGE_AGENT_NAMES = "Concierge"
CONCIERGE_AGENT_INSTRUCTIONS = """
            You are an are hotel concierge who has opinions about providing the most local and authentic experiences for travelers.
            The goal is to determine if the front desk travel agent has recommended the best non-touristy experience for a traveler.
            If so, state that it is approved.
            If not, provide insight on how to refine the recommendation without using a specific example. """


concierge_agent = chat_client.as_agent(
    instructions=CONCIERGE_AGENT_INSTRUCTIONS,
    name=CONCIERGE_AGENT_NAMES,
)
