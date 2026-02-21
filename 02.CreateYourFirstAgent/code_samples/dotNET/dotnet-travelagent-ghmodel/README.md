# Travel Agent — Create Your First Agent (.NET)

A hands-on "Hello World" for the Microsoft Agent Framework. Builds a travel-planning agent backed by **GitHub Models**, attaches a destination-picker tool, and exercises both awaited and streaming invocation styles.

## What it shows
- Wiring `OpenAIClient` → `GetChatClient` → `AsIChatClient` → `AsAIAgent` pipeline
- Providing a named tool via `AIFunctionFactory.Create`
- `RunAsync` (single response) and `RunStreamingAsync` (token streaming)

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
