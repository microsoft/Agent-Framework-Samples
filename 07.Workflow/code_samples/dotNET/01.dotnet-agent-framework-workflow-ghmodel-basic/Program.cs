using System;
using System.ComponentModel;
using System.ClientModel;
using OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using DotNetEnv;

// Load environment variables
Env.Load("../../../../.env");

// Get GitHub configuration
var github_endpoint = Environment.GetEnvironmentVariable("GITHUB_ENDPOINT") ?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
var github_model_id = Environment.GetEnvironmentVariable("GITHUB_MODEL_ID") ?? "gpt-4o-mini";
var github_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");

// Configure OpenAI client
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri(github_endpoint)
};

var openAIClient = new OpenAIClient(new ApiKeyCredential(github_token), openAIOptions);

// Define agent configurations
const string ReviewerAgentName = "Concierge";
const string ReviewerAgentInstructions = @"
    You are an are hotel concierge who has opinions about providing the most local and authentic experiences for travelers.
    The goal is to determine if the front desk travel agent has recommended the best non-touristy experience for a traveler.
    If so, state that it is approved.
    If not, provide insight on how to refine the recommendation without using a specific example. 
    ";

const string FrontDeskAgentName = "FrontDesk";
const string FrontDeskAgentInstructions = @"
    You are a Front Desk Travel Agent with ten years of experience and are known for brevity as you deal with many customers.
    The goal is to provide the best activities and locations for a traveler to visit.
    Only provide a single recommendation per response.
    You're laser focused on the goal at hand.
    Don't waste time with chit chat.
    Consider suggestions when refining an idea.
    ";

// Create AI agents
AIAgent reviewerAgent = openAIClient.GetChatClient(github_model_id).AsIChatClient().AsAIAgent(
    name: ReviewerAgentName, instructions: ReviewerAgentInstructions);
AIAgent frontDeskAgent = openAIClient.GetChatClient(github_model_id).AsIChatClient().AsAIAgent(
    name: FrontDeskAgentName, instructions: FrontDeskAgentInstructions);

// Build workflow
var workflow = new WorkflowBuilder(frontDeskAgent)
    .AddEdge(frontDeskAgent, reviewerAgent)
    .Build();

// Create user message
ChatMessage userMessage = new ChatMessage(ChatRole.User, [
    new TextContent("I would like to go to Paris.")
]);

// Execute workflow
StreamingRun run = await InProcessExecution.StreamAsync(workflow, userMessage);

// Process workflow events
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

string messageData = "";

await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is AgentResponseUpdateEvent executorComplete)
    {
        messageData += executorComplete.Data;
        Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
    }
}

Console.WriteLine("\n=== Final Output ===");
Console.WriteLine(messageData);

// Mermaid
Console.WriteLine("\nMermaid string: \n=======");
var mermaid = workflow.ToMermaidString();
Console.WriteLine(mermaid);
Console.WriteLine("=======");

// DOT - Save to file instead of stdout to avoid pipe issues
var dotString = workflow.ToDotString();
var dotFilePath = "workflow.dot";
File.WriteAllText(dotFilePath, dotString);
Console.WriteLine($"\nDOT graph saved to: {dotFilePath}");
Console.WriteLine("To generate image: dot -Tsvg workflow.dot -o workflow.svg");
Console.WriteLine("                   dot -Tpng workflow.dot -o workflow.png");

// Console.WriteLine(messageData);


