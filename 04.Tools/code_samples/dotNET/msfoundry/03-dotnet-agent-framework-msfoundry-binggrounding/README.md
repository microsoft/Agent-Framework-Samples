# Bing Grounding Tool — Azure AI Foundry (.NET)

Connects an Azure AI Foundry agent to a **Bing Grounding** connection so it can search the live web and cite sources in answers. Demonstrates how to attach a `BingGroundingAgentTool` to a Foundry-hosted agent.

## What it shows
- Looking up an `AIProjectConnection` by name and wrapping it in `BingGroundingAgentTool`
- Attaching real-time web search capability to a Foundry agent
- Responses that reference live Bing search results

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed model
- A **Bing Search** connection configured in your AI Foundry hub
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
dotnet user-secrets set "BING_CONNECTION_NAME" "<your-bing-connection-name>"
```

## Run

```bash
dotnet run
```
