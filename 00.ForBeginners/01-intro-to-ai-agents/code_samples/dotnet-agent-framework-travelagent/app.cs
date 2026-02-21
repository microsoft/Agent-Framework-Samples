// File-based run: dotnet run app.cs
#:property UserSecretsId e01f82ab-60d0-4d0e-82b0-476cd48c55b3
#:package Microsoft.Extensions.AI@10.3.0
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package OpenAI@2.8.0
#:package Microsoft.Agents.AI@1.0.0-rc1
#:package Microsoft.Agents.AI.OpenAI@1.0.0-rc1
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0

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
	?? throw new InvalidOperationException("GITHUB_ENDPOINT is required");
var github_model_id = config["GITHUB_MODEL_ID"] ?? "gpt-4o-mini";
var github_token = config["GITHUB_TOKEN"] 
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
		name: "TravelAgent",
		instructions: "You are a helpful AI Agent that can help plan vacations for customers at random destinations.",
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
