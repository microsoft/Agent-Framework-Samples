# MCP Tool with Approval Flow — Azure AI Foundry (.NET)

Connects a Foundry agent to a remote **Model Context Protocol (MCP)** server (`microsoft_learn`) and requires explicit user approval before each tool call. Demonstrates how to implement a human-in-the-loop approval gate for MCP-based tool invocations.

## What it shows
- Attaching a `HostedMcpServerTool` to a Foundry agent with `ApprovalMode.AlwaysRequire`
- Processing `McpServerToolApprovalRequestContent` messages in the response loop
- Submitting user approval decisions back to the agent with `McpServerToolApprovalResponseContent`

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed model
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
dotnet user-secrets set "AZURE_AI_MODEL_DEPLOYMENT_NAME" "gpt-4.1-mini"
```

## Run

```bash
dotnet run
```

When the agent wants to call an MCP tool, it will prompt you to enter `Y` to approve before the call is made.

## Run with file-based approach (.NET 10)

```bash
dotnet run app.cs
```
