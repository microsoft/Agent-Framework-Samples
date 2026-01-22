"""Publisher Agent Configuration - Azure AI Foundry with Code Interpreter Tool"""

from pydantic import BaseModel

# Agent configuration
PUBLISHER_NAME = "publisher-agent"
PUBLISHER_INSTRUCTIONS = """
You are the content publisher. Run code to save the tutorial's draft content as a Markdown file. Saved file's name is marked with current date and time, such as yearmonthdayhourminsec. Note that if it is 1-9, you need to add 0, such as 20240101123045.md.
Set 'file_path' to save path. Always return result as JSON with fields 'file_path' (string).
"""


class PublisherAgent(BaseModel):
    file_path: str
