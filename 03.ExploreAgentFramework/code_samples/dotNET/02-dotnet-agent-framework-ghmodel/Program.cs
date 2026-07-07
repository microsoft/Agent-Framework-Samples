using System;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using OpenAI;
using DotNetEnv;

// Load environment variables from .env file
Env.Load("../../../../.env");

// Get configuration from environment variables
var github_endpoint = Environment.GetEnvironmentVariable("GITHUB_ENDPOINT") 
    ?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
var github_model_id = Environment.GetEnvironmentVariable("GITHUB_MODEL_ID") ?? "gpt-4o-mini";
var github_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") 
    ?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");

// Configure OpenAI client for GitHub Models
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri(github_endpoint)
};

var openAIClient = new OpenAIClient(new ApiKeyCredential(github_token), openAIOptions);

// Create AI Agent
AIAgent agent = openAIClient
    .GetChatClient(github_model_id)
    .AsIChatClient()
    .AsAIAgent(instructions: "You are a helpful assistant.", name: "dotNET");

// Run agent with standard response
Console.WriteLine("=== Standard Response ===");
Console.WriteLine(await agent.RunAsync("Write a haiku about Agent Framework."));

// Run agent with streaming response
Console.WriteLine("\n=== Streaming Response ===");
await foreach (var update in agent.RunStreamingAsync("Write a haiku about Agent Framework."))
{
    Console.Write(update);
}
Console.WriteLine();
