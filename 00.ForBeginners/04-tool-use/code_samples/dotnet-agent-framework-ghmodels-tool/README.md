# Tool Use (.NET)

Shows how to build a stateful agent conversation using `AgentSession`. A travel-planning agent uses a custom tool to suggest destinations across multiple sequential turns, demonstrating how session context is preserved between calls.

## What it shows
- Creating and managing an `AgentSession` for multi-turn conversations
- Registering and invoking a C# function as an agent tool
- How the agent autonomously decides when to call a tool based on user input

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
