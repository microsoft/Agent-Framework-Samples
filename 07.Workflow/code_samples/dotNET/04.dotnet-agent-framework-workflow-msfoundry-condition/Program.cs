using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var endpoint = config["AZURE_AI_PROJECT_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var deploymentName = config["AZURE_AI_MODEL_DEPLOYMENT_NAME"] ?? "gpt-4o-mini";

// Create an Azure AI Foundry project client
AIProjectClient aiProjectClient = new(new Uri(endpoint), new AzureCliCredential());

// Create four specialist agents as Azure AI Foundry persistent agents
AIAgent writerAgent = await aiProjectClient.CreateAIAgentAsync(
    name: "Writer-Agent",
    creationOptions: new AgentVersionCreationOptions(
        new PromptAgentDefinition(model: deploymentName)
        {
            Instructions = """
                You are a creative travel content writer.
                When given a destination, write a concise, engaging 2-3 sentence paragraph about it.
                Focus on what makes it unique and appealing to visitors.
                """
        }));

AIAgent reviewerAgent = await aiProjectClient.CreateAIAgentAsync(
    name: "Reviewer-Agent",
    creationOptions: new AgentVersionCreationOptions(
        new PromptAgentDefinition(model: deploymentName)
        {
            Instructions = """
                You are a strict travel content quality reviewer.
                Review the content and decide whether it is ready to publish.
                Your first word MUST be either "APPROVED" or "REVISE" followed by your feedback.
                Example: "APPROVED: Great descriptions, ready to publish."
                Example: "REVISE: Missing specific landmarks and local details."
                """
        }));

AIAgent editorAgent = await aiProjectClient.CreateAIAgentAsync(
    name: "Editor-Agent",
    creationOptions: new AgentVersionCreationOptions(
        new PromptAgentDefinition(model: deploymentName)
        {
            Instructions = """
                You are a travel content editor.
                You receive content flagged for revision. Improve it by adding specific landmarks,
                local cuisine highlights, and vivid sensory details. Return only the improved paragraph.
                """
        }));

AIAgent publisherAgent = await aiProjectClient.CreateAIAgentAsync(
    name: "Publisher-Agent",
    creationOptions: new AgentVersionCreationOptions(
        new PromptAgentDefinition(model: deploymentName)
        {
            Instructions = """
                You are a travel content publisher.
                Format the final content with a bold heading and return the polished result.
                """
        }));

// Condition predicates that inspect the reviewer agent's ChatMessage output
static bool IsApproved(ChatMessage? msg) =>
    msg?.Text?.Contains("APPROVED", StringComparison.OrdinalIgnoreCase) == true;

static bool NeedsRevision(ChatMessage? msg) =>
    msg?.Text?.Contains("REVISE", StringComparison.OrdinalIgnoreCase) == true;

// Build the conditional workflow:
//
//   writerAgent → reviewerAgent
//                    ↓ [APPROVED] ──────────────── publisherAgent
//                    ↓ [REVISE]   → editorAgent → publisherAgent
//
var workflow = new WorkflowBuilder(writerAgent)
    .AddEdge(writerAgent, reviewerAgent)
    .AddEdge<ChatMessage>(reviewerAgent, publisherAgent, IsApproved, label: "approved")
    .AddEdge<ChatMessage>(reviewerAgent, editorAgent, NeedsRevision, label: "needs revision")
    .AddEdge(editorAgent, publisherAgent)
    .Build();

Console.WriteLine("=== Conditional Content Workflow — Azure AI Foundry ===\n");

// Execute the workflow with a travel destination as input
await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
    workflow,
    new ChatMessage(ChatRole.User, "Write travel content about Tokyo, Japan."));
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is AgentResponseUpdateEvent update)
    {
        Console.Write($"[{update.ExecutorId}] {update.Data}");
    }
}

Console.WriteLine("\n");

// Visualize the workflow graph
Console.WriteLine("Workflow Mermaid:\n=======");
Console.WriteLine(workflow.ToMermaidString());
Console.WriteLine("=======\n");

var dotFilePath = "workflow.dot";
File.WriteAllText(dotFilePath, workflow.ToDotString());
Console.WriteLine($"DOT graph saved to: {dotFilePath}");
Console.WriteLine("To generate image: dot -Tpng workflow.dot -o workflow.png");
