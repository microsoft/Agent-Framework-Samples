import asyncio
import json
import os
from typing import Any

import nest_asyncio

from agent_framework_foundry_local import FoundryLocalClient
from azure.ai.evaluation.red_team import AttackStrategy, RedTeam, RiskCategory
from azure.identity import AzureCliCredential
from dotenv import load_dotenv

load_dotenv()
nest_asyncio.apply()

async def main() -> None:
    """Run a red team evaluation."""
    print("\n" + "=" * 80)
    print("RED TEAM EVALUATION")
    print("=" * 80 + "\n")

    # Initialize Azure credentials
    credential = AzureCliCredential()
    # Create the agent
    agent = FoundryLocalClient(model_id="qwen2.5-1.5b-instruct-generic-cpu:4").as_agent(
        name="LocalAgent",
        instructions="""You are an assistant.

Your responsibilities:
- Answering questions and providing professional advice
- Helping users understand concepts
- Offering users different suggestions

""",
    )

    # Create the callback - must be synchronous for RedTeam
    def agent_callback(query: str) -> str:
        """Callback function that interfaces between RedTeam and the agent.

        Args:
            query: The adversarial prompt from RedTeam
        
        Returns:
            The agent's response as a string
        """
        async def _run():
            return await agent.run(query)
        
        try:
            # Run the async agent call in a new event loop
            response = asyncio.get_event_loop().run_until_complete(_run())
            return response.text

        except Exception as e:
            print(f"Error during agent run: {e}")
            return f"I encountered an error and couldn't process your request: {e!s}"

    # Create RedTeam instance
    red_team = RedTeam(
        azure_ai_project=os.environ["AZURE_AI_PROJECT_ENDPOINT"],
        credential=credential,
        risk_categories=[
            RiskCategory.Violence,
            RiskCategory.HateUnfairness,
            RiskCategory.Sexual,
            RiskCategory.SelfHarm,
        ],
        num_objectives=2,  # Small number for quick testing
    )

    print("Running basic red team evaluation...")
    print("Risk Categories: Violence, HateUnfairness, Sexual, SelfHarm")
    print("Attack Objectives per category: 5")
    print("Attack Strategy: Baseline (unmodified prompts)\n")

    # Run the red team evaluation
    results = await red_team.scan(
        target=agent_callback,
        scan_name="Qwen2.5-1.5B-Agent",
        attack_strategies=[
            AttackStrategy.EASY,  # Group of easy complexity attacks
            AttackStrategy.MODERATE,  # Group of moderate complexity attacks
            AttackStrategy.CharacterSpace,  # Add character spaces
            AttackStrategy.ROT13,  # Use ROT13 encoding
            AttackStrategy.UnicodeConfusable,  # Use confusable Unicode characters
            AttackStrategy.CharSwap,  # Swap characters in prompts
            AttackStrategy.Morse,  # Encode prompts in Morse code
            AttackStrategy.Leetspeak,  # Use Leetspeak
            AttackStrategy.Url,  # Use URLs in prompts
            AttackStrategy.Binary,  # Encode prompts in binary
            AttackStrategy.Compose([AttackStrategy.Base64, AttackStrategy.ROT13]),  # Use two strategies in one attack
        ],
        output_path="Qwen2.5-1.5B-Redteam-Results.json",
    )

    # Display results
    print("\n" + "-" * 80)
    print("EVALUATION RESULTS")
    print("-" * 80)
    print(json.dumps(results.to_scorecard(), indent=2))


if __name__ == "__main__":
    asyncio.run(main())