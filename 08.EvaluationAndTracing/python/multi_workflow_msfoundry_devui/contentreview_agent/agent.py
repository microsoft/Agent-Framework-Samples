"""Content Reviewer Agent Configuration - Azure AI Foundry Basic Agent"""

from pydantic import BaseModel
from typing_extensions import Literal

# Agent configuration
REVIEWER_NAME = "reviewer-agent"
REVIEWER_INSTRUCTIONS = """
You are a content reviewer for a publishing company. You need to check whether the tutorial's draft content meets the following requirements:

1. If the draft content is less than 200 words, set 'review_result' to 'No' and 'reason' to 'Content is too short'. If the draft content is more than 200 words, set 'review_result' to 'Yes' and 'reason' to 'The content is good'.
2. Set 'draft_content' to the original draft content.
3. Return result as JSON with fields 'review_result' (one of 'Yes', 'No'), 'reason' (string), and 'draft_content' (string).
"""


class ReviewAgent(BaseModel):
    review_result: Literal["Yes", "No"]
    reason: str
    draft_content: str
