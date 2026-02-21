using System;
using System.ComponentModel;
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

var github_endpoint = config["GITHUB_ENDPOINT"] 
	?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
var github_model_id = config["GITHUB_MODEL_ID"] ?? "gpt-4o-mini";
var github_token = config["GITHUB_TOKEN"] 
	?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");

// Configure OpenAI client for GitHub Models
var openAIOptions = new OpenAIClientOptions()
{
	Endpoint = new Uri(github_endpoint)
};

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
AIAgent agent = new OpenAIClient(new ApiKeyCredential(github_token), openAIOptions)
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
