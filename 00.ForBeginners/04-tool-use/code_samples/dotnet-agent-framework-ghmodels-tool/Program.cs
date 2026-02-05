using System;
using System.ComponentModel;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using OpenAI;
using DotNetEnv;

// Load environment variables from .env file
Env.Load("../../../../.env");

// Tool: Random Destination Generator
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

// Get GitHub Models configuration from environment variables
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

// Create AI Agent with tool integration
AIAgent agent = openAIClient.GetChatClient(github_model_id).AsIChatClient().AsAIAgent(
	instructions: "You are a helpful AI Agent that can help plan vacations for customers at random destinations.",
	tools: [AIFunctionFactory.Create((Func<string>)GetRandomDestination)]
);

// Create conversation thread for context
// AgentThread thread = await agent.GetNewThreadAsync(); 
AgentSession session = await agent.CreateSessionAsync();

// Run agent with tool invocation
Console.WriteLine(await agent.RunAsync("Plan me a day trip", session));

// Follow-up request to demonstrate tool re-invocation
Console.WriteLine(await agent.RunAsync("I don't like that destination. Plan me another vacation.", session));
