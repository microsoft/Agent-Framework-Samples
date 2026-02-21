# Basic Agent — Agentic Design Patterns (.NET)

Illustrates common agentic design patterns: giving an agent a specific role through system instructions and equipping it with a domain-specific tool. The same agent is invoked two ways to compare response styles.

## What it shows
- Role-based agent instructions (system prompt)
- Tool attachment and automatic tool-call dispatch
- Standard vs. streaming response patterns

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
