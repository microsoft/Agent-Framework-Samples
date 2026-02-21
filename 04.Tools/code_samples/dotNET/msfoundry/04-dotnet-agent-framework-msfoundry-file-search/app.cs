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

// See https://aka.ms/new-console-template for more information
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

var endpoint = config["AZURE_AI_PROJECT_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var deploymentName = config["AZURE_AI_MODEL_DEPLOYMENT_NAME"] ?? "gpt-4o-mini";

// Create an AI Project client and get an OpenAI client that works with the foundry service.
AIProjectClient aiProjectClient = new(
    new Uri(endpoint),
    new AzureCliCredential());
OpenAIClient openAIClient = aiProjectClient.GetProjectOpenAIClient();

// Upload the file that contains the data to be used for RAG to the Foundry service.
OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
ClientResult<OpenAIFile> uploadResult = await fileClient.UploadFileAsync(
    filePath: "../../../files/demo.md",
    purpose: FileUploadPurpose.Assistants);

#pragma warning disable OPENAI001
VectorStoreClient vectorStoreClient = openAIClient.GetVectorStoreClient();
ClientResult<VectorStore> vectorStoreCreate = await vectorStoreClient.CreateVectorStoreAsync(options: new VectorStoreCreationOptions()
{
    Name = "rag-document-knowledge-base",
    FileIds = { uploadResult.Value.Id }
});
#pragma warning restore OPENAI001

var fileSearchTool = new HostedFileSearchTool() { Inputs = [new HostedVectorStoreContent(vectorStoreCreate.Value.Id)] };

AIAgent agent = await aiProjectClient
    .CreateAIAgentAsync(
        model: deploymentName,
        name: "dotNETRAGAgent",
        instructions: @"You are an AI assistant that helps people find information in a set of documents. Use the File Search tool to look up relevant information from the files when needed to answer user questions. If you don't know the answer, just say you don't know. Do not make up answers.",
        tools: [fileSearchTool]);


AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine(await agent.RunAsync("What's graphrag?", session));
