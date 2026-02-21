# RAG — File Search with Azure AI Foundry (.NET)

Uploads a Markdown knowledge-base document to Azure AI Foundry and uses `HostedFileSearchTool` to build a retrieval-augmented generation (RAG) pipeline. The agent answers questions using only the indexed document.

## What it shows
- End-to-end RAG pattern: file upload → vector store creation → `HostedFileSearchTool` attachment → grounded Q&A
- Strict grounding instructions that prevent the agent from using general knowledge
- `AgentSession` for multi-turn document Q&A

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed model
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
dotnet user-secrets set "AZURE_AI_MODEL_DEPLOYMENT_NAME" "gpt-4o-mini"
```

> The sample uploads `../../files/demo.md` (located in the `06.RAGs/code_samples/` folder).

## Run

```bash
dotnet run
```
