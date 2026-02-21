# Basic Workflow — Two-Agent Review Loop (.NET)

Builds a two-agent workflow using `WorkflowBuilder`: a **FrontDesk** travel agent proposes activity recommendations and a **Concierge** reviewer accepts or requests refinements. The agents take turns until the concierge approves.

## What it shows
- Building a directed agent graph with `WorkflowBuilder.AddEdge`
- Running a workflow with `InProcessExecution.RunStreamingAsync`
- Consuming `WorkflowEvent` / `AgentResponseUpdateEvent` updates in a streaming loop

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
