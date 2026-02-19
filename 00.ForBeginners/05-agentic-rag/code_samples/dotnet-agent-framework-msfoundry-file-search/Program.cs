using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Reflection;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Files;
using OpenAI.VectorStores;
using DotNetEnv;


Env.Load("../../../../.env");

var endpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT") ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_AI_MODEL_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

// Create an AI Project client and get an OpenAI client that works with the foundry service.
AIProjectClient aiProjectClient = new(
    new Uri(endpoint),
    new AzureCliCredential());
OpenAIClient openAIClient = aiProjectClient.GetProjectOpenAIClient();

// Upload the file that contains the data to be used for RAG to the Foundry service.
OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
ClientResult<OpenAIFile> uploadResult;
try
{
    uploadResult = await fileClient.UploadFileAsync(
        filePath: "../document.md",
        purpose: FileUploadPurpose.Assistants);
}
catch (ClientResultException ex)
{
    LogDetailedClientError("File upload failed", ex);
    throw;
}

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

//Console.WriteLine(await agent.RunAsync("Can you explain Contoso's travel insurance coverage?", session));
Console.WriteLine(await agent.RunAsync("What is the weather in Ohio today?", session));

static void LogDetailedClientError(string context, ClientResultException ex)
{
    Console.Error.WriteLine($"[Client Error] {context}");
    Console.Error.WriteLine($"Status: {ex.Status}");
    Console.Error.WriteLine($"Message: {ex.Message}");
    var responseBody = TryExtractResponseBody(ex);
    if (!string.IsNullOrWhiteSpace(responseBody))
    {
        Console.Error.WriteLine("Server response body:");
        Console.Error.WriteLine(responseBody);
    }
}

static string? TryExtractResponseBody(ClientResultException ex)
{
    try
    {
        var responseField = typeof(ClientResultException).GetField("_response", BindingFlags.NonPublic | BindingFlags.Instance);
        if (responseField?.GetValue(ex) is PipelineResponse response)
        {
            var stream = response.ContentStream;
            if (stream == null)
            {
                return null;
            }

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using var reader = new StreamReader(stream, leaveOpen: true);
            var body = reader.ReadToEnd();

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return body;
        }
    }
    catch
    {
        // Ignore reflection or IO errors when extracting diagnostics.
    }

    return null;
}

