# Concurrent Workflow — Fan-Out / Fan-In Pattern (.NET)

Demonstrates a parallel (fan-out) workflow: a **Researcher** and a **Planner** agent run concurrently on the same input, and a `ConcurrentAggregationExecutor` merges their outputs before producing a final combined travel plan.

## What it shows
- `WorkflowBuilder.AddFanOutEdge` to dispatch to multiple agents simultaneously
- `AddFanInBarrierEdge` with `ConcurrentAggregationExecutor` to merge parallel outputs
- `ConcurrentStartExecutor` as the workflow entry point

> **Note:** `ReflectingExecutor<T>` and `IMessageHandler<T>` used in this sample are marked obsolete. They will be replaced with `[MessageHandler]`-attributed partial classes in a future version.

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- A [GitHub Models](https://github.com/marketplace/models) personal access token with access to `gpt-4o`

## Configure secrets

```bash
dotnet user-secrets set "GITHUB_TOKEN" "<your-github-pat>"
dotnet user-secrets set "GITHUB_ENDPOINT" "https://models.inference.ai.azure.com"
```

> The model is hardcoded to `gpt-4o`.

## Run

```bash
dotnet run
```
