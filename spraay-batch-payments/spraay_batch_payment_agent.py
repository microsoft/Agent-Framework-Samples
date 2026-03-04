"""
Spraay Batch Payment Agent - Microsoft Agent Framework Sample

This sample demonstrates how to build an AI agent that can batch-send
ETH and ERC-20 tokens to multiple recipients on Base using the Spraay
protocol. The agent uses function tools to execute blockchain transactions
with ~80% gas savings compared to individual transfers.

Prerequisites:
    pip install agent-framework --pre web3

Environment Variables:
    OPENAI_API_KEY          - OpenAI API key (or use Azure OpenAI)
    OPENAI_CHAT_MODEL_ID    - Model to use (default: gpt-4o)
    SPRAAY_PRIVATE_KEY      - Wallet private key for signing transactions
    SPRAAY_RPC_URL          - RPC endpoint (default: https://mainnet.base.org)

Usage:
    python spraay_batch_payment_agent.py
"""

import asyncio
import json
import os
from typing import Annotated, List

from pydantic import Field

from agent_framework import Agent
from agent_framework.openai import OpenAIChatClient

# ---------------------------------------------------------------------------
# Spraay contract configuration
# ---------------------------------------------------------------------------

SPRAAY_CONTRACT = "0x1646452F98E36A3c9Cfc3eDD8868221E207B5eEC"
DEFAULT_RPC = "https://mainnet.base.org"
BASE_CHAIN_ID = 8453

SPRAAY_ABI = json.loads("""[
    {
        "inputs": [
            {"internalType": "address[]", "name": "recipients", "type": "address[]"},
            {"internalType": "uint256", "name": "amountPerRecipient", "type": "uint256"}
        ],
        "name": "batchSendETH",
        "outputs": [],
        "stateMutability": "payable",
        "type": "function"
    },
    {
        "inputs": [
            {"internalType": "address", "name": "token", "type": "address"},
            {"internalType": "address[]", "name": "recipients", "type": "address[]"},
            {"internalType": "uint256", "name": "amountPerRecipient", "type": "uint256"}
        ],
        "name": "batchSendToken",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function"
    },
    {
        "inputs": [
            {"internalType": "address[]", "name": "recipients", "type": "address[]"},
            {"internalType": "uint256[]", "name": "amounts", "type": "uint256[]"}
        ],
        "name": "batchSendETHVariable",
        "outputs": [],
        "stateMutability": "payable",
        "type": "function"
    },
    {
        "inputs": [
            {"internalType": "address", "name": "token", "type": "address"},
            {"internalType": "address[]", "name": "recipients", "type": "address[]"},
            {"internalType": "uint256[]", "name": "amounts", "type": "uint256[]"}
        ],
        "name": "batchSendTokenVariable",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function"
    }
]""")

ERC20_APPROVE_ABI = json.loads("""[
    {
        "inputs": [
            {"internalType": "address", "name": "spender", "type": "address"},
            {"internalType": "uint256", "name": "amount", "type": "uint256"}
        ],
        "name": "approve",
        "outputs": [{"internalType": "bool", "name": "", "type": "bool"}],
        "stateMutability": "nonpayable",
        "type": "function"
    }
]""")


# ---------------------------------------------------------------------------
# Web3 helpers
# ---------------------------------------------------------------------------

def _get_web3_connection():
    """Initialize web3 connection and account from environment."""
    from web3 import Web3

    rpc_url = os.environ.get("SPRAAY_RPC_URL", DEFAULT_RPC)
    private_key = os.environ.get("SPRAAY_PRIVATE_KEY")
    if not private_key:
        raise ValueError("SPRAAY_PRIVATE_KEY environment variable is required.")

    w3 = Web3(Web3.HTTPProvider(rpc_url))
    account = w3.eth.account.from_key(private_key)
    contract = w3.eth.contract(
        address=Web3.to_checksum_address(SPRAAY_CONTRACT),
        abi=SPRAAY_ABI,
    )
    return w3, account, contract


# ---------------------------------------------------------------------------
# Function tools — these are plain Python functions that the Agent Framework
# automatically exposes to the LLM as callable tools.
# ---------------------------------------------------------------------------

def batch_send_eth(
    recipients: Annotated[List[str], Field(description="List of recipient wallet addresses (max 200)")],
    amount_per_recipient_eth: Annotated[str, Field(description="ETH amount to send to each recipient, e.g. '0.01'")],
) -> str:
    """Batch send equal amounts of ETH to multiple recipients on Base using Spraay.
    Saves ~80% on gas vs individual transfers. Max 200 recipients per transaction."""
    try:
        from web3 import Web3

        w3, account, contract = _get_web3_connection()

        if len(recipients) > 200:
            return "Error: Maximum 200 recipients per transaction."

        checksummed = [Web3.to_checksum_address(r) for r in recipients]
        amount_wei = w3.to_wei(amount_per_recipient_eth, "ether")
        fee = amount_wei * 30 // 10000  # 0.3% protocol fee
        total_value = (amount_wei + fee) * len(checksummed)

        tx = contract.functions.batchSendETH(
            checksummed, amount_wei
        ).build_transaction({
            "from": account.address,
            "value": total_value,
            "nonce": w3.eth.get_transaction_count(account.address),
            "chainId": BASE_CHAIN_ID,
        })

        signed = account.sign_transaction(tx)
        tx_hash = w3.eth.send_raw_transaction(signed.raw_transaction)
        receipt = w3.eth.wait_for_transaction_receipt(tx_hash)

        return (
            f"Success! Sent {amount_per_recipient_eth} ETH to "
            f"{len(checksummed)} recipients. "
            f"Tx: https://basescan.org/tx/{receipt['transactionHash'].hex()}"
        )
    except Exception as e:
        return f"Error: {e}"


def batch_send_token(
    token_address: Annotated[str, Field(description="ERC-20 token contract address")],
    recipients: Annotated[List[str], Field(description="List of recipient wallet addresses (max 200)")],
    amount_per_recipient: Annotated[str, Field(description="Token amount per recipient in human-readable units, e.g. '100'")],
    token_decimals: Annotated[int, Field(description="Token decimal places (default 18)")] = 18,
) -> str:
    """Batch send equal amounts of an ERC-20 token to multiple recipients on Base.
    Handles token approval automatically. Max 200 recipients."""
    try:
        from web3 import Web3

        w3, account, contract = _get_web3_connection()

        if len(recipients) > 200:
            return "Error: Maximum 200 recipients per transaction."

        checksummed = [Web3.to_checksum_address(r) for r in recipients]
        token_addr = Web3.to_checksum_address(token_address)
        amount_raw = int(float(amount_per_recipient) * (10 ** token_decimals))
        total_amount = amount_raw * len(checksummed)

        # Approve tokens
        token_contract = w3.eth.contract(address=token_addr, abi=ERC20_APPROVE_ABI)
        approve_tx = token_contract.functions.approve(
            Web3.to_checksum_address(SPRAAY_CONTRACT), total_amount
        ).build_transaction({
            "from": account.address,
            "nonce": w3.eth.get_transaction_count(account.address),
            "chainId": BASE_CHAIN_ID,
        })
        signed_approve = account.sign_transaction(approve_tx)
        approve_hash = w3.eth.send_raw_transaction(signed_approve.raw_transaction)
        w3.eth.wait_for_transaction_receipt(approve_hash)

        # Batch send
        tx = contract.functions.batchSendToken(
            token_addr, checksummed, amount_raw
        ).build_transaction({
            "from": account.address,
            "nonce": w3.eth.get_transaction_count(account.address),
            "chainId": BASE_CHAIN_ID,
        })
        signed = account.sign_transaction(tx)
        tx_hash = w3.eth.send_raw_transaction(signed.raw_transaction)
        receipt = w3.eth.wait_for_transaction_receipt(tx_hash)

        return (
            f"Success! Sent {amount_per_recipient} tokens to "
            f"{len(checksummed)} recipients. "
            f"Tx: https://basescan.org/tx/{receipt['transactionHash'].hex()}"
        )
    except Exception as e:
        return f"Error: {e}"


def batch_send_eth_variable(
    recipients: Annotated[List[str], Field(description="List of recipient wallet addresses (max 200)")],
    amounts_eth: Annotated[List[str], Field(description="List of ETH amounts for each recipient, e.g. ['0.01', '0.05']")],
) -> str:
    """Batch send different amounts of ETH to each recipient on Base.
    Useful for payroll or bounty distribution with varying amounts."""
    try:
        from web3 import Web3

        w3, account, contract = _get_web3_connection()

        if len(recipients) != len(amounts_eth):
            return "Error: recipients and amounts_eth must have the same length."
        if len(recipients) > 200:
            return "Error: Maximum 200 recipients per transaction."

        checksummed = [Web3.to_checksum_address(r) for r in recipients]
        amounts_wei = [w3.to_wei(a, "ether") for a in amounts_eth]
        fees = [a * 30 // 10000 for a in amounts_wei]
        total_value = sum(a + f for a, f in zip(amounts_wei, fees))

        tx = contract.functions.batchSendETHVariable(
            checksummed, amounts_wei
        ).build_transaction({
            "from": account.address,
            "value": total_value,
            "nonce": w3.eth.get_transaction_count(account.address),
            "chainId": BASE_CHAIN_ID,
        })

        signed = account.sign_transaction(tx)
        tx_hash = w3.eth.send_raw_transaction(signed.raw_transaction)
        receipt = w3.eth.wait_for_transaction_receipt(tx_hash)

        total_eth = sum(float(a) for a in amounts_eth)
        return (
            f"Success! Sent {total_eth} ETH total to "
            f"{len(checksummed)} recipients (variable amounts). "
            f"Tx: https://basescan.org/tx/{receipt['transactionHash'].hex()}"
        )
    except Exception as e:
        return f"Error: {e}"


def batch_send_token_variable(
    token_address: Annotated[str, Field(description="ERC-20 token contract address")],
    recipients: Annotated[List[str], Field(description="List of recipient wallet addresses (max 200)")],
    amounts: Annotated[List[str], Field(description="List of token amounts for each recipient")],
    token_decimals: Annotated[int, Field(description="Token decimal places (default 18)")] = 18,
) -> str:
    """Batch send different amounts of an ERC-20 token to each recipient on Base.
    Handles token approval automatically."""
    try:
        from web3 import Web3

        w3, account, contract = _get_web3_connection()

        if len(recipients) != len(amounts):
            return "Error: recipients and amounts must have the same length."
        if len(recipients) > 200:
            return "Error: Maximum 200 recipients per transaction."

        checksummed = [Web3.to_checksum_address(r) for r in recipients]
        token_addr = Web3.to_checksum_address(token_address)
        amounts_raw = [int(float(a) * (10 ** token_decimals)) for a in amounts]
        total_amount = sum(amounts_raw)

        # Approve
        token_contract = w3.eth.contract(address=token_addr, abi=ERC20_APPROVE_ABI)
        approve_tx = token_contract.functions.approve(
            Web3.to_checksum_address(SPRAAY_CONTRACT), total_amount
        ).build_transaction({
            "from": account.address,
            "nonce": w3.eth.get_transaction_count(account.address),
            "chainId": BASE_CHAIN_ID,
        })
        signed_approve = account.sign_transaction(approve_tx)
        approve_hash = w3.eth.send_raw_transaction(signed_approve.raw_transaction)
        w3.eth.wait_for_transaction_receipt(approve_hash)

        # Batch send
        tx = contract.functions.batchSendTokenVariable(
            token_addr, checksummed, amounts_raw
        ).build_transaction({
            "from": account.address,
            "nonce": w3.eth.get_transaction_count(account.address),
            "chainId": BASE_CHAIN_ID,
        })
        signed = account.sign_transaction(tx)
        tx_hash = w3.eth.send_raw_transaction(signed.raw_transaction)
        receipt = w3.eth.wait_for_transaction_receipt(tx_hash)

        total_tokens = sum(float(a) for a in amounts)
        return (
            f"Success! Sent {total_tokens} tokens total to "
            f"{len(checksummed)} recipients (variable amounts). "
            f"Tx: https://basescan.org/tx/{receipt['transactionHash'].hex()}"
        )
    except Exception as e:
        return f"Error: {e}"


# ---------------------------------------------------------------------------
# Agent definition
# ---------------------------------------------------------------------------

async def main():
    agent = Agent(
        name="SpraayPaymentAgent",
        client=OpenAIChatClient(),
        instructions="""You are a blockchain payment assistant that helps users
batch-send ETH and ERC-20 tokens to multiple recipients on the Base network
using the Spraay protocol.

You have access to four payment tools:
- batch_send_eth: Send equal ETH amounts to multiple recipients
- batch_send_token: Send equal token amounts to multiple recipients
- batch_send_eth_variable: Send different ETH amounts to each recipient
- batch_send_token_variable: Send different token amounts to each recipient

Key facts about Spraay:
- Max 200 recipients per transaction
- ~80% gas savings vs individual transfers
- 0.3% protocol fee
- Deployed on Base (chain ID 8453)
- Common tokens on Base: USDC (0x833589fCD6eDb6E08f4c7C32D4f71b54bdA02913, 6 decimals)

When a user wants to send payments:
1. Confirm the recipients and amounts
2. Choose the appropriate tool (equal vs variable amounts, ETH vs token)
3. Execute the transaction and return the BaseScan link
""",
        tools=[
            batch_send_eth,
            batch_send_token,
            batch_send_eth_variable,
            batch_send_token_variable,
        ],
    )

    # Example interaction
    result = await agent.run(
        "I need to send 0.01 ETH to these 3 addresses: "
        "0x742d35Cc6634C0532925a3b844Bc9e7595f2bD18, "
        "0x53d284357ec70cE289D6D64134DfAc8E511c8a3D, "
        "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2"
    )
    print(result.text)


if __name__ == "__main__":
    asyncio.run(main())
