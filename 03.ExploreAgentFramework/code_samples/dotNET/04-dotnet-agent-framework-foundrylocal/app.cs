// File-based run: dotnet run app.cs
#:property UserSecretsId 54f947d6-b3f4-4d15-9772-afd04450d9d3
#:package Microsoft.Extensions.AI@10.3.0
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package OpenAI@2.8.0
#:package Microsoft.Agents.AI@1.0.0-rc1
#:package Microsoft.Agents.AI.AzureAI@1.0.0-rc1
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0

using System;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using OpenAI;
using Microsoft.Extensions.Configuration;

// Load environment variables from .env file

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var foundryLocalEndpoint = config["FOUNDRYLOCAL_ENDPOINT"] 
    ?? throw new InvalidOperationException("FOUNDRYLOCAL_ENDPOINT is not set.");
var foundryLocalModelId = config["FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME"] 
    ?? throw new InvalidOperationException("FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME is not set.");

Console.WriteLine($"Endpoint: {foundryLocalEndpoint}");
Console.WriteLine($"Model: {foundryLocalModelId}");

// Configure OpenAI client for Foundry Local (no API key required)
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri(foundryLocalEndpoint)
};

var openAIClient = new OpenAIClient(new ApiKeyCredential("nokey"), openAIOptions);

// Create AI Agent
AIAgent agent = openAIClient
    .GetChatClient(foundryLocalModelId)
    .AsIChatClient()
    .AsAIAgent(instructions: "You are a helpful assistant.", name: "FoundryLocalAgent");

// Run agent
Console.WriteLine("\n=== Response ===");
Console.WriteLine(await agent.RunAsync("Can you introduce yourself?"));

// Run agent with streaming response
Console.WriteLine("\n=== Streaming Response ===");
await foreach (var update in agent.RunStreamingAsync("What can you help me with?"))
{
    Console.Write(update);
}
Console.WriteLine();
