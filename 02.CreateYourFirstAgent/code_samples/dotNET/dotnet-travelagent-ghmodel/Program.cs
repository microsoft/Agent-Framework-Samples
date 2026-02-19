using System;
using System.ComponentModel;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Azure.AI.OpenAI;
using DotNetEnv;

// Load environment variables from .env file
Env.Load("../../../../.env");

// Get GitHub Models configuration from environment variables
var github_endpoint = Environment.GetEnvironmentVariable("GITHUB_ENDPOINT") 
    ?? throw new InvalidOperationException("GITHUB_ENDPOINT is required");
var github_model_id = Environment.GetEnvironmentVariable("GITHUB_MODEL_ID") ?? "gpt-4o-mini";
var github_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") 
    ?? throw new InvalidOperationException("GITHUB_TOKEN is required");

// Configure OpenAI client for GitHub Models
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri(github_endpoint)
};

// Create AI Agent with custom tool
AIAgent agent = new OpenAIClient(new ApiKeyCredential(github_token), openAIOptions)
    .GetChatClient(github_model_id)
    .AsIChatClient()
    .AsAIAgent(
        name: "TravelPlanAgent",
        instructions: "You are a helpful AI Agent that can help plan vacations for customers at random destinations",
        tools: [AIFunctionFactory.Create((Func<string>)GetRandomDestination)]
    );

// Run agent with standard response
Console.WriteLine("=== Travel Plan ===");
Console.WriteLine(await agent.RunAsync("Plan me a day trip"));

// Run agent with streaming response
Console.WriteLine("\n=== Streaming Travel Plan ===");
await foreach (var update in agent.RunStreamingAsync("Plan me a day trip"))
{
    Console.Write(update);
}
Console.WriteLine();

// Agent Tool: Random Destination Generator
[Description("Provides a random vacation destination for travel planning.")]
static string GetRandomDestination()
{
    var destinations = new List<string>
    {
        "Paris, France",
        "Tokyo, Japan",
        "New York City, USA",
        "Sydney, Australia",
        "Rome, Italy",
        "Barcelona, Spain",
        "Cape Town, South Africa",
        "Rio de Janeiro, Brazil",
        "Bangkok, Thailand",
        "Vancouver, Canada"
    };

    var random = new Random();
    int index = random.Next(destinations.Count);
    return destinations[index];
}
