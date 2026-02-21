# Travel Agent — Intro to AI Agents (.NET)

Introduces the `AIAgent` abstraction by building a travel-planning agent with a custom tool that returns random destination suggestions. Demonstrates both a standard (awaited) response and a streaming response.

## What it shows
- Creating an `AIAgent` from an `OpenAI` chat client targeting **GitHub Models**
- Registering a C# method as an agent tool with `AIFunctionFactory.Create`
- Calling `RunAsync` for a single response and `RunStreamingAsync` for token-by-token streaming

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
