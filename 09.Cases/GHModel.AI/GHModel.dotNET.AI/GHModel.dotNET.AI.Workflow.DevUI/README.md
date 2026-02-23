# Agent DevUI — Case Study (.NET)

An ASP.NET Core web application that hosts the **Agent Framework DevUI** for interactive browser-based agent testing. This case-study variant is paired with the OpenTelemetry and AGUI samples in the same solution to demonstrate a complete agent observability story.

## What it shows
- Hosting the DevUI alongside other ASP.NET services in a multi-project solution
- Registering agents and workflows via `Microsoft.Agents.AI.Hosting`
- `MapDevUI()` and `MapOpenAIResponses()` for full DevUI functionality

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
