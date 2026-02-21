// File-based run: dotnet run app.cs
#:property UserSecretsId 8324968c-859a-4090-8afa-336940d7cf66
#:property EnablePreviewFeatures true
#:package Azure.AI.OpenAI@2.1.0
#:package Azure.Identity@1.17.1
#:package Azure.AI.Projects@1.2.0-beta.5
#:package Azure.AI.Projects.OpenAI@1.0.0-beta.5
#:package Azure.AI.Agents.Persistent@1.2.0-beta.8
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Agents.AI@1.0.0-rc1
#:package Microsoft.Agents.AI.AzureAI@1.0.0-rc1
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0

// File-based run: dotnet run app.cs
#:property UserSecretsId 8324968c-859a-4090-8afa-336940d7cf66
#:property EnablePreviewFeatures true
#:package Azure.AI.OpenAI@2.1.0
#:package Azure.Identity@1.17.1
#:package Azure.AI.Projects@1.2.0-beta.5
#:package Azure.AI.Projects.OpenAI@1.0.0-beta.5
#:package Azure.AI.Agents.Persistent@1.2.0-beta.8
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Agents.AI@1.0.0-rc1
#:package Microsoft.Agents.AI.AzureAI@1.0.0-rc1
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0

// ============================================================
//  04 — FILE SEARCH TOOL
//  This sample shows how HostedFileSearchTool plugs into an
//  AIAgent using the exact same pattern as the other hosted
//  tools in this chapter (Code Interpreter, Bing Search):
//
//    1. Prepare the tool  — upload a file + create a vector store
//    2. Create the tool   — new HostedFileSearchTool(vectorStoreId)
//    3. Register the tool — pass it in the tools: [] parameter
//
//  The agent framework handles calling the tool automatically.
//  For a full RAG pipeline walkthrough, see 06.RAGs.
// ============================================================

using System.ClientModel;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Files;
using OpenAI.VectorStores;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var endpoint      = config["AZURE_AI_PROJECT_ENDPOINT"]        ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var deploymentName = config["AZURE_AI_MODEL_DEPLOYMENT_NAME"] ?? "gpt-4o-mini";

// Step 1 — Prepare the tool: upload the knowledge file and index it into a vector store.
// The vector store is the searchable index that backs the File Search tool.
AIProjectClient aiProjectClient = new(new Uri(endpoint), new AzureCliCredential());
OpenAIClient    openAIClient    = aiProjectClient.GetProjectOpenAIClient();

OpenAIFileClient fileClient    = openAIClient.GetOpenAIFileClient();
var              uploadResult  = await fileClient.UploadFileAsync(
    filePath: "../../../files/demo.md",
    purpose: FileUploadPurpose.Assistants);

#pragma warning disable OPENAI001
VectorStoreClient vectorStoreClient = openAIClient.GetVectorStoreClient();
var               vectorStoreResult = await vectorStoreClient.CreateVectorStoreAsync(
    options: new VectorStoreCreationOptions()
    {
        Name    = "file-search-tool-demo",
        FileIds = { uploadResult.Value.Id }
    });
#pragma warning restore OPENAI001

// Step 2 — Create the tool.
// HostedFileSearchTool follows the same pattern as HostedCodeInterpreterTool
// and HostedWebSearchTool shown in earlier samples in this chapter.
var fileSearchTool = new HostedFileSearchTool()
{
    Inputs = [new HostedVectorStoreContent(vectorStoreResult.Value.Id)]
};

// Step 3 — Register the tool with the agent.
// The agent framework decides when to invoke the tool based on the user's query.
AIAgent agent = await aiProjectClient.CreateAIAgentAsync(
    model:        deploymentName,
    name:         "FileSearchToolAgent",
    instructions: "You are a helpful assistant. Use the File Search tool to find information in the uploaded documents. If you can't find the answer, say so clearly.",
    tools:        [fileSearchTool]);

AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine("=== File Search Tool Demo ===");
Console.WriteLine(await agent.RunAsync("What is GraphRAG and what is it used for?", session));
