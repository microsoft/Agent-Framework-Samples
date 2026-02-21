# Vision Tool — Azure AI Foundry (.NET)

Demonstrates multimodal input: an image of a room is sent to the agent, which analyses the visible furniture and provides purchase recommendations. Uses `DataContent` to attach image bytes to a `ChatMessage`.

## What it shows
- Sending images to an agent via `DataContent` (base-64 encoded bytes) in a `ChatMessage`
- Creating a Foundry-hosted agent configured for multimodal (`gpt-4o`) inference
- Maintaining a session (`AgentSession`) across a multi-turn visual conversation

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project with a `gpt-4o` deployment
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
```

> The model is hardcoded to `gpt-4o` because vision requires a multimodal model. The image used is `../../../files/home.png` (relative to the project folder).

## Run

```bash
dotnet run
```
