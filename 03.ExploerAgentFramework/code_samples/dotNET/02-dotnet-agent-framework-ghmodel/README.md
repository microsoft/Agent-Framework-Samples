# Agent Framework — GitHub Models Provider (.NET)

Minimal sample showing `AIAgent` creation with **GitHub Models** as the provider. Demonstrates that switching providers only requires changing the `IChatClient` — the agent API stays the same.

## What it shows
- Creating an `AIAgent` from `OpenAIClient` pointed at `models.inference.ai.azure.com`
- Provider-agnostic `RunAsync` / `RunStreamingAsync` invocation
- How `AsIChatClient()` adapts the OpenAI SDK chat client to `Microsoft.Extensions.AI`

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
