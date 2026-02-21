# Agent Framework — Azure AI Foundry Provider (.NET)

Creates a managed agent on **Azure AI Foundry** via `AIProjectClient.CreateAIAgentAsync` using `AgentVersionCreationOptions` / `PromptAgentDefinition`. The agent is versioned and reusable across sessions.

## What it shows
- Using `AIProjectClient` to create and retrieve versioned agents in Foundry
- `AgentVersionCreationOptions` with `PromptAgentDefinition` for declarative agent setup
- Invoking a Foundry-hosted agent with `RunAsync`

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
