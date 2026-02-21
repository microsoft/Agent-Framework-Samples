# Conditional Workflow — Azure AI Foundry (.NET)

Demonstrates **conditional edge routing** using Azure AI Foundry and the Microsoft Agent Framework. A reviewer agent inspects content and routes it along different workflow paths depending on its decision.

## What it demonstrates
- Defining conditional edges with `AddEdge<T>(source, target, condition)` that branch based on agent output
- A **content quality gate**: the reviewer outputs `APPROVED` or `REVISE`, routing to different downstream agents
- Azure AI Foundry persistent agents (`CreateAIAgentAsync`) as the backing provider

## Workflow

```
writerAgent → reviewerAgent
                  ↓ [APPROVED] ──────────────── publisherAgent
                  ↓ [REVISE]   → editorAgent → publisherAgent
```

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed model
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
dotnet user-secrets set "AZURE_AI_MODEL_DEPLOYMENT_NAME" "gpt-4o-mini"
```

## Run

```bash
dotnet run
```

## Run with file-based approach (.NET 10)

```bash
dotnet run app.cs
```
