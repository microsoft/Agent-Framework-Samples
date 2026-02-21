# Planning Design (.NET)

Demonstrates the planning agentic design pattern. A planner agent receives a high-level travel request and produces a structured `TravelPlan` JSON object that breaks the task into specialised sub-agent assignments (flights, hotels, activities, etc.).

## What it shows
- Structured JSON output via `ChatResponseFormatJson` and `AIJsonUtilities.CreateJsonSchema`
- Using an agent as an orchestrator / router that decomposes tasks for specialist agents
- Configuring `ChatClientAgentOptions` with a response format schema

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

## Run with file-based approach (.NET 10)

```bash
dotnet run app.cs
```
