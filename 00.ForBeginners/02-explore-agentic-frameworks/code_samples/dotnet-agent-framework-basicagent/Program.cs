using System;
using System.ComponentModel;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Azure;
using Azure.AI.OpenAI;
using DotNetEnv;

// Load environment variables from .env file
Env.Load("../../../../.env");

// Get GitHub Models configuration from environment variables
var github_endpoint = Environment.GetEnvironmentVariable("GITHUB_ENDPOINT") 
	?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
var github_model_id = Environment.GetEnvironmentVariable("GITHUB_MODEL_ID") ?? "gpt-4o-mini";
var github_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") 
	?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");


// Agent Tool: Random Destination Generator
[Description("Provides a random vacation destination.")]
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

// Create AI Agent with basic instructions and tool
AIAgent agent = new AzureOpenAIClient(new Uri(github_endpoint), new AzureKeyCredential(github_token))
	.GetChatClient(github_model_id)
	.AsIChatClient()
	.AsAIAgent(
		name: "BasicAgent",
		instructions: "You are a basic AI Agent that can answer questions and suggest random travel destinations.",
		tools: [AIFunctionFactory.Create((Func<string>)GetRandomDestination)]
	);

// Run agent with standard response
Console.WriteLine("=== Basic Agent Response ===");
Console.WriteLine(await agent.RunAsync("Suggest a vacation destination."));

// Run agent with streaming response
Console.WriteLine("\n=== Streaming Response ===");
await foreach (var update in agent.RunStreamingAsync("Suggest a vacation destination."))
{
	Console.Write(update);
}
Console.WriteLine();
