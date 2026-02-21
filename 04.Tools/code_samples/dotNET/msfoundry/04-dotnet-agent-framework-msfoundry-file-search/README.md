# File Search Tool — Azure AI Foundry (.NET)

Uploads a Markdown document to Azure AI Foundry, indexes it in a vector store, and attaches `HostedFileSearchTool` to the agent. The agent answers questions grounded exclusively in the uploaded file content.

## What it shows
- Uploading files to Foundry with `OpenAIFileClient.UploadFileAsync`
- Creating a vector store and linking it via `HostedVectorStoreContent`
- Using `HostedFileSearchTool` for retrieval-augmented generation (RAG)

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An [Azure AI Foundry](https://ai.azure.com) project with a deployed model
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_AI_PROJECT_ENDPOINT" "<your-foundry-project-endpoint>"
dotnet user-secrets set "AZURE_AI_MODEL_DEPLOYMENT_NAME" "gpt-4o-mini"
```

> The sample uploads `../../../files/demo.md` (located in the `04.Tools/code_samples/` folder).

## Run

```bash
dotnet run
```

## Run with file-based approach (.NET 10)

```bash
dotnet run app.cs
```
