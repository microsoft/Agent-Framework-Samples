# Spraay Batch Payment Agent

A Microsoft Agent Framework sample that demonstrates how to build an AI agent for batch cryptocurrency payments using the [Spraay](https://spraay.app) protocol on Base.

## What This Sample Shows

- **Custom function tools**: Four blockchain payment tools using Python type annotations and Pydantic `Field` descriptions
- **Real-world tool integration**: Connecting an AI agent to live smart contracts via web3.py
- **Transaction handling**: Building, signing, and broadcasting Ethereum transactions from an agent

## Features

| Tool | Description |
|------|-------------|
| `batch_send_eth` | Send equal ETH amounts to multiple recipients |
| `batch_send_token` | Send equal ERC-20 token amounts (with auto-approval) |
| `batch_send_eth_variable` | Send different ETH amounts to each recipient |
| `batch_send_token_variable` | Send different token amounts to each recipient |

**Key benefits of Spraay:**
- Up to 200 recipients per transaction
- ~80% gas savings vs individual transfers
- 0.3% protocol fee
- Live on Base mainnet

## Prerequisites

```bash
pip install agent-framework --pre web3
```

## Environment Variables

```bash
# LLM Provider (choose one)
export OPENAI_API_KEY="sk-..."
export OPENAI_CHAT_MODEL_ID="gpt-4o"  # optional, defaults to gpt-4o

# Spraay (required)
export SPRAAY_PRIVATE_KEY="your-wallet-private-key"

# Optional
export SPRAAY_RPC_URL="https://mainnet.base.org"  # default
```

## Running the Sample

```bash
python spraay_batch_payment_agent.py
```

The agent will process a sample batch ETH payment request. You can modify the `main()` function to test different scenarios:

```python
# Equal ETH distribution
result = await agent.run("Send 0.01 ETH to 0xAbc... and 0xDef...")

# USDC distribution
result = await agent.run(
    "Send 50 USDC to these addresses: 0xAbc..., 0xDef... "
    "USDC address is 0x833589fCD6eDb6E08f4c7C32D4f71b54bdA02913 with 6 decimals"
)

# Variable payroll
result = await agent.run(
    "Pay my team: 0xAbc gets 0.05 ETH, 0xDef gets 0.03 ETH, 0x123 gets 0.07 ETH"
)
```

## Architecture

```
User Request
    │
    ▼
┌─────────────────────┐
│  Agent Framework     │
│  (OpenAI / Azure)    │
│                      │
│  Instructions +      │
│  Tool Descriptions   │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  Function Tools      │
│  batch_send_eth()    │
│  batch_send_token()  │
│  ...                 │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  web3.py             │
│  Build + Sign Tx     │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  Spraay Contract     │
│  Base Mainnet        │
│  0x1646...7eEC       │
└─────────────────────┘
```

## Links

- **Spraay App**: https://spraay.app
- **Spraay Dapp**: https://spraay-base-dapp.vercel.app
- **Contract**: [BaseScan](https://basescan.org/address/0x1646452F98E36A3c9Cfc3eDD8868221E207B5eEC)
- **MCP Server**: Available on [Smithery](https://smithery.ai/server/@AnonJr/spraay-mcp-server) and 7+ directories
- **x402 Gateway**: https://gateway.spraay.app (9 paid API endpoints for AI agents)
- **Microsoft Agent Framework**: https://github.com/microsoft/agent-framework
