using System;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using OpenAI;
using DotNetEnv;

// Load environment variables from .env file
Env.Load("../../../../.env");

// Get Foundry Local configuration from environment variables
var foundryLocalEndpoint = Environment.GetEnvironmentVariable("FOUNDRYLOCAL_ENDPOINT") 
    ?? throw new InvalidOperationException("FOUNDRYLOCAL_ENDPOINT is not set.");
var foundryLocalModelId = Environment.GetEnvironmentVariable("FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME") 
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
