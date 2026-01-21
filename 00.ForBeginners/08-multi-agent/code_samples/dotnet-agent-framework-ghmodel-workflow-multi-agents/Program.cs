using System;
using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using OpenAI;
using DotNetEnv;

// Load environment variables from .env file
Env.Load("../../../.env");

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

// Define Concierge Agent (Quality Reviewer)
const string REVIEWER_NAME = "Concierge";
const string REVIEWER_INSTRUCTIONS = @"""
    You are an are hotel concierge who has opinions about providing the most local and authentic experiences for travelers.
    The goal is to determine if the front desk travel agent has recommended the best non-touristy experience for a traveler.
    If so, state that it is approved.
    If not, provide insight on how to refine the recommendation without using a specific example. 
    """;

// Define Front Desk Agent (Travel Specialist)
const string FRONTDESK_NAME = "FrontDesk";
const string FRONTDESK_INSTRUCTIONS = @"""
    You are a Front Desk Travel Agent with ten years of experience and are known for brevity as you deal with many customers.
    The goal is to provide the best activities and locations for a traveler to visit.
    Only provide a single recommendation per response.
    You're laser focused on the goal at hand.
    Don't waste time with chit chat.
    Consider suggestions when refining an idea.
    """;

// Configure agent options
ChatClientAgentOptions frontdeskAgentOptions = new()
{
    Name = FRONTDESK_NAME,
    Description = FRONTDESK_INSTRUCTIONS
};

ChatClientAgentOptions reviewerAgentOptions = new()
{
    Name = REVIEWER_NAME,
    Description = REVIEWER_INSTRUCTIONS
};

// Create agent instances
AIAgent reviewerAgent = openAIClient.GetChatClient(github_model_id).AsIChatClient().AsAIAgent(reviewerAgentOptions);
AIAgent frontdeskAgent = openAIClient.GetChatClient(github_model_id).AsIChatClient().AsAIAgent(frontdeskAgentOptions);

// Build the multi-agent workflow
var workflow = new WorkflowBuilder(frontdeskAgent)
    .AddEdge(frontdeskAgent, reviewerAgent)
    .Build();

// Start the streaming workflow execution
StreamingRun run = await InProcessExecution.StreamAsync(workflow, new ChatMessage(ChatRole.User, "I would like to go to Paris."));

// Send message to start workflow
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

// Process streaming workflow events
string strResult = "";
await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is AgentResponseUpdateEvent executorComplete)
    {
        strResult += executorComplete.Data;
        Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
    }
}

// Display final aggregated result
Console.WriteLine("\n=== Final Result ===");
Console.WriteLine(strResult);
