# Code Interpreter Tool — Azure AI Foundry (.NET)

Attaches the hosted **Code Interpreter** tool to a Foundry agent, which lets it write and execute Python code server-side to answer computational questions. The sample captures and displays both the generated code and the execution output.

## What it shows
- Enabling `ResponseTool.CreateCodeInterpreterTool` on a Foundry agent
- Reading `CodeInterpreterToolCallContent` and `CodeInterpreterToolResultContent` from the response
- How the agent autonomously writes and runs code to solve problems (Fibonacci sequence)

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed model
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
```

> The model is hardcoded to `gpt-4.1-mini`.

## Run

```bash
dotnet run
```

## Run with file-based approach (.NET 10)

```bash
dotnet run app.cs
```
