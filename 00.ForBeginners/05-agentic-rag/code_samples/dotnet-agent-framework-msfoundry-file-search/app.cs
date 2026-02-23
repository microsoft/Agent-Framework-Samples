// File-based run: dotnet run app.cs
#:property UserSecretsId 4e72b0fc-2c2e-4371-9d75-0912883af3f6
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
    filePath: "../document.md",
    purpose: FileUploadPurpose.Assistants);

#pragma warning disable OPENAI001
VectorStoreClient vectorStoreClient = openAIClient.GetVectorStoreClient();
ClientResult<VectorStore> vectorStoreCreate = await vectorStoreClient.CreateVectorStoreAsync(options: new VectorStoreCreationOptions()
{
    Name = "document-knowledge-base",
    FileIds = { uploadResult.Value.Id }
});
#pragma warning restore OPENAI001

var fileSearchTool = new HostedFileSearchTool() { Inputs = [new HostedVectorStoreContent(vectorStoreCreate.Value.Id)] };

AIAgent agent = await aiProjectClient
    .CreateAIAgentAsync(
        model: deploymentName,
        name: "dotNETRAGAgent",
        instructions: @"You are an AI assistant designed to answer user questions using only the information retrieved from the provided document(s). 
                If a user's question cannot be answered using the retrieved context, you must clearly respond: 
                'I'm sorry, but the uploaded document does not contain the necessary information to answer that question.' 
                Do not answer from general knowledge or reasoning. Do not make assumptions or generate hypothetical explanations. 
                For questions that do have relevant content in the document, respond accurately and cite the document explicitly.",
        tools: [fileSearchTool]);

AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine(await agent.RunAsync("Can you explain Contoso's travel insurance coverage?", session));
