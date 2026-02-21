# Basic Agent — Exploring Agentic Frameworks (.NET)

Demonstrates how an agentic framework wraps a chat client to produce an autonomous agent. A destination-suggestion tool is attached to the agent and invoked automatically during the conversation.

## What it shows
- Constructing an `AIAgent` with a name, instructions, and a tool
- How the Agent Framework handles the tool-calling loop transparently
- Side-by-side standard and streaming responses

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- A [GitHub Models](https://github.com/marketplace/models) personal access token

## Configure secrets

```bash
dotnet user-secrets set "GITHUB_TOKEN" "<your-github-pat>"
dotnet user-secrets set "GITHUB_ENDPOINT" "https://models.inference.ai.azure.com"
dotnet user-secrets set "GITHUB_MODEL_ID" "gpt-4o-mini"
```

## Run

```bash
dotnet run
```
