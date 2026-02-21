# RAG — File Search with Azure AI Foundry (.NET)

> **Maintainer note:** This sample shares the same agent pattern as [`00.ForBeginners/05-agentic-rag/…/dotnet-agent-framework-msfoundry-file-search`](../../../../00.ForBeginners/05-agentic-rag/code_samples/dotnet-agent-framework-msfoundry-file-search/). The two differ only in the uploaded document (`demo.md` here vs. `document.md` there) and the seed question. Consider converging them on a single document or expanding this chapter-level sample with additional RAG patterns (e.g., hybrid search, metadata filters).

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
