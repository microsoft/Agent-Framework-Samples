# Multi-Agent Workflow (.NET)

Shows how to connect two agents in a workflow: a **FrontDesk** travel agent proposes recommendations and a **Concierge** reviewer approves or refines them. The workflow runs iteratively until the concierge approves.

## What it shows
- Building a two-agent review loop with `WorkflowBuilder`
- Defining distinct roles and personas via `ChatClientAgentOptions`
- Running a workflow with `InProcessExecution.RunStreamingAsync` and consuming `WorkflowEvent` updates

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
