using System;
using Azure;
using Azure.AI.OpenAI;
using DotNetEnv;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;


// Load environment variables from .env file
Env.Load("../../../../.env");

// Get Azure OpenAI configuration from environment variables
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
	?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT")
	?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT is not set.");
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")
	?? throw new InvalidOperationException("AZURE_OPENAI_KEY is not set.");

// Configure Azure OpenAI client for the specified deployment
var azureOpenAIClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

// Planning agent configuration
const string AGENT_NAME = "TravelPlanAgent";
const string AGENT_INSTRUCTIONS = @"You are an planner agent.
	Your job is to decide which agents to run based on the user's request.
	Below are the available agents specialised in different tasks:
	- FlightBooking: For booking flights and providing flight information
	- HotelBooking: For booking hotels and providing hotel information
	- CarRental: For booking cars and providing car rental information
	- ActivitiesBooking: For booking activities and providing activity information
	- DestinationInfo: For providing information about destinations
	- DefaultAgent: For handling general request";

ChatClientAgentOptions agentOptions = new()
{
	Name = AGENT_NAME,
	Description = AGENT_INSTRUCTIONS,
	ChatOptions = new()
	{
		ResponseFormat = ChatResponseFormatJson.ForJsonSchema(
			schema: AIJsonUtilities.CreateJsonSchema(typeof(TravelPlan)),
			schemaName: "TravelPlan",
			schemaDescription: "Travel Plan with main_task and subtasks")
	}
};

AIAgent agent = azureOpenAIClient
	.GetChatClient(deploymentName)
	.AsIChatClient()
	.AsAIAgent(agentOptions);

Console.WriteLine(await agent.RunAsync("Create a travel plan for a family of 4, with 2 kids, from Singapore to Melbourne"));
