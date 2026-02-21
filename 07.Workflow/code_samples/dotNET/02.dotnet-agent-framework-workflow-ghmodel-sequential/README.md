# Sequential Workflow — Image-to-Quote Pipeline (.NET)

Constructs a three-stage sequential workflow: a **Sales Agent** identifies furniture from a room image, a **Price Agent** estimates costs, and a **Quote Agent** produces a formatted Markdown purchase quote. Demonstrates multimodal input fed into a linear agent chain.

## What it shows
- A multi-stage sequential pipeline with `WorkflowBuilder`
- Passing image bytes (`DataContent`) as initial workflow input
- Each agent receiving the previous agent's output as context

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- A [GitHub Models](https://github.com/marketplace/models) personal access token with access to `gpt-4o`

## Configure secrets

```bash
dotnet user-secrets set "GITHUB_TOKEN" "<your-github-pat>"
dotnet user-secrets set "GITHUB_ENDPOINT" "https://models.inference.ai.azure.com"
```

> The model is hardcoded to `gpt-4o`. The image used is `../../imgs/home.png` (relative to the project folder).

## Run

```bash
dotnet run
```

## Run with file-based approach (.NET 10)

```bash
dotnet run app.cs
```
