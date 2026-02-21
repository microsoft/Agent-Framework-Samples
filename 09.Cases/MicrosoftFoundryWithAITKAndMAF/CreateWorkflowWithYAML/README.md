# Declarative Workflow with YAML — Azure AI Foundry (.NET)

Loads a multi-agent workflow from a YAML definition file using `WorkflowFactory` and runs it with `WorkflowRunner`. Demonstrates how workflows can be defined declaratively without writing C# wiring code.

## What it shows
- Using `WorkflowFactory` to deserialise a YAML workflow definition at runtime
- `WorkflowRunner.ExecuteAsync` to run a declaratively-defined workflow
- Separation of workflow topology (YAML) from host application code (C#)

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed model
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
```

Optionally, point to a custom YAML file:

```bash
dotnet user-secrets set "WORKFLOW_YAML_PATH" "path/to/your-workflow.yaml"
```

The default YAML files (`workflow.yaml`, `apply_agent.yaml`, `hiring_manager_agent.yaml`) are located in the [`YAML/`](../YAML/) folder and are copied to the output directory at build time.

## Run

```bash
dotnet run
```
