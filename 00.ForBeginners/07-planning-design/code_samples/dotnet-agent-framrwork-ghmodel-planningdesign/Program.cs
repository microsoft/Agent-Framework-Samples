using System;
using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using OpenAI;
using DotNetEnv;


// Load environment variables from .env file
Env.Load("../../../../.env");

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

AIAgent agent = openAIClient.GetChatClient(github_model_id).AsIChatClient().AsAIAgent(agentOptions);

Console.WriteLine(await agent.RunAsync("Create a travel plan for a family of 4, with 2 kids, from Singapore to Melbourne"));
