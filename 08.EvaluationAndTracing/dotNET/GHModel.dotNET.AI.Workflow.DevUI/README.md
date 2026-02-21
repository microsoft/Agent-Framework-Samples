# Agent DevUI — Evaluation & Tracing (.NET)

An ASP.NET Core web application that hosts the **Agent Framework DevUI** — an interactive browser-based interface for testing, debugging, and tracing agent workflows without writing a test harness.

## What it shows
- Adding the DevUI to an ASP.NET Core app with `MapDevUI()`
- Registering agents and workflows via `Microsoft.Agents.AI.Hosting`
- `MapOpenAIResponses()` for Python DevUI cross-compatibility (dynamic model routing)
- Browsing agent conversation history and individual message events in the UI

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- A [GitHub Models](https://github.com/marketplace/models) personal access token

## Configure secrets

```bash
dotnet user-secrets set "GITHUB_TOKEN" "<your-github-pat>"
dotnet user-secrets set "GITHUB_ENDPOINT" "https://models.inference.ai.azure.com"
```

## Run

```bash
dotnet run
```

Open **http://localhost:50518/devui** in your browser to access the DevUI.
