# Agent Framework — Foundry Local Provider (.NET)

Runs an `AIAgent` entirely on-device using **Foundry Local** — no cloud endpoint or API key required (an empty `"nokey"` credential is accepted). Demonstrates that the same agent API works with locally-hosted models.

## What it shows
- Connecting to a local Foundry endpoint with `OpenAIClient`
- Running an agent without any cloud dependency
- Standard and streaming responses from a local model

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- [Foundry Local](https://github.com/microsoft/foundry-local) installed and running with a downloaded model

## Configure secrets

```bash
dotnet user-secrets set "FOUNDRYLOCAL_ENDPOINT" "http://localhost:5272/v1"
dotnet user-secrets set "FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME" "<model-alias>"
```

> Run `foundry model list` to view downloaded model aliases.

## Run

```bash
dotnet run
```
