# AGUI Server — AG-UI Protocol (.NET)

An ASP.NET Core server that exposes agent workflows over the **AG-UI (Agent-User Interaction) protocol**. The server registers agents via `AddAGUI()` and maps their endpoints so any AG-UI-compatible client can stream conversation turns.

## What it shows
- Setting up an AG-UI server with `builder.Services.AddAGUI()` and `app.MapAGUI()`
- `ChatClientAgentFactory` for provider-agnostic agent construction from configuration
- Streaming agent responses over the AG-UI protocol to connected clients

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

The server listens on **http://localhost:5018**. Start this before running the companion [AGUI.Client](../GHModel.dotNET.AI.Workflow.AGUI.Client/).
