# Agentic RAG — File Search with Azure AI Foundry (.NET)

Uploads a document to Azure AI Foundry, creates a vector store from it, and attaches a `HostedFileSearchTool` to the agent. The agent answers questions using only the uploaded document's content.

## What it shows
- Uploading files to Azure AI Foundry and creating a vector store
- Attaching `HostedFileSearchTool` for retrieval-augmented generation (RAG)
- Grounding an agent's responses strictly in the uploaded document

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
dotnet user-secrets set "AZURE_AI_MODEL_DEPLOYMENT_NAME" "gpt-4o-mini"
```

## Run

```bash
dotnet run
```

> The sample uploads `../document.md` (located in the `05-agentic-rag/code_samples/` folder) to Foundry at startup.

## Run with file-based approach (.NET 10)

```bash
dotnet run app.cs
```
