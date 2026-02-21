# Conditional Workflow — Azure AI Foundry (.NET)

Placeholder for a conditional branching workflow backed by **Azure AI Foundry**. The workflow will route agent execution to different paths based on run-time conditions evaluated by the workflow engine.

> **Status:** Implementation in progress. Run the project to see the scaffold output.

## What it will show
- Defining conditional edges in `WorkflowBuilder` that branch based on agent output
- Routing messages to different specialist agents depending on a decision condition
- Azure AI Foundry as the backing provider for a multi-agent conditional workflow

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
