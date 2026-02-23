// ============================================================
//  AGENT-TO-AGENT (A2A) PATTERN
//
//  In A2A, one agent treats another agent as a callable provider —
//  a specialist it can delegate to on demand.
//
//  This sample shows two agents:
//    • ResearchAgent  — a specialist that returns concise facts
//    • OrchestratorAgent — wraps ResearchAgent as a tool and calls
//      it automatically when it needs research to answer the user
//
//  The key mechanism:
//    AIFunctionFactory.Create(researchFunc, ...) — wraps the
//    ResearchAgent's RunAsync as an AIFunction tool that the
//    orchestrator can invoke just like any other tool.
//
//  Compare with 07.Workflow where agents are wired into a fixed
//  pipeline. Here the orchestrator decides at runtime whether and
//  when to call the specialist agent.
// ============================================================

using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var githubEndpoint = config["GITHUB_ENDPOINT"] ?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
var githubModel    = config["GITHUB_MODEL_ID"]  ?? "gpt-4o-mini";
var githubToken    = config["GITHUB_TOKEN"]     ?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");

var openAIOptions = new OpenAIClientOptions { Endpoint = new Uri(githubEndpoint) };
IChatClient chatClient = new OpenAIClient(new ApiKeyCredential(githubToken), openAIOptions)
    .GetChatClient(githubModel)
    .AsIChatClient();

// -----------------------------------------------
// Agent 1 — ResearchAgent (the specialist provider)
// Focused, terse instructions: return facts only,
// no elaboration. This keeps the delegation output
// clean and easy for the orchestrator to re-use.
// -----------------------------------------------
AIAgent researchAgent = chatClient.AsAIAgent(
    name: "ResearchAgent",
    instructions: """
        You are a concise research specialist. When given a topic or question,
        respond with exactly 3-5 factual bullet points and nothing else.
        No introductions, no conclusions, no extra commentary.
        """);

// -----------------------------------------------
// A2A Bridge — wrap the ResearchAgent as a tool
//
// AIFunctionFactory.Create turns any .NET delegate into an AIFunction
// that a language model can call. By wrapping ResearchAgent.RunAsync
// here, the OrchestratorAgent can invoke it as a tool call — the
// same mechanism used for APIs and database queries.
//
// From the orchestrator's perspective, ResearchAgent is just another
// provider it can query; it does not know or care that the provider
// is itself an AI agent.
// -----------------------------------------------
Func<string, Task<string>> researchFunc =
    async (query) => (await researchAgent.RunAsync(query)).ToString();

var researchTool = AIFunctionFactory.Create(
    (Func<string, Task<string>>)researchFunc,
    name: "ResearchTopic",
    description: "Delegates a research query to the ResearchAgent specialist. " +
                 "Returns 3-5 concise factual bullet points about the topic.");

// -----------------------------------------------
// Agent 2 — OrchestratorAgent (the coordinator)
// The orchestrator knows it has a ResearchTopic tool
// and is instructed to use it before drafting an answer.
// It never does the research itself.
// -----------------------------------------------
AIAgent orchestratorAgent = chatClient.AsAIAgent(
    name: "OrchestratorAgent",
    instructions: """
        You are a travel guide assistant. For every destination question:
        1. ALWAYS call ResearchTopic first to get factual details.
        2. Use the returned facts to build a 2-3 sentence travel tip.
        3. Keep the tone friendly and engaging.
        Never answer from your own knowledge without calling ResearchTopic.
        """,
    tools: [researchTool]);

// -----------------------------------------------
// Run — the orchestrator transparently calls the
// ResearchAgent behind the scenes for each question.
// -----------------------------------------------
Console.WriteLine("=== Agent-to-Agent (A2A) Demo ===");
Console.WriteLine();

// Turn 1 — orchestrator will call ResearchAgent for Tokyo facts
Console.WriteLine("Q: What should I know about visiting Tokyo?");
var answer1 = await orchestratorAgent.RunAsync("What should I know about visiting Tokyo?");
Console.WriteLine($"A: {answer1}");
Console.WriteLine();

// Turn 2 — orchestrator calls ResearchAgent again for a different topic
Console.WriteLine("Q: Give me a travel tip for Cape Town.");
var answer2 = await orchestratorAgent.RunAsync("Give me a travel tip for Cape Town.");
Console.WriteLine($"A: {answer2}");
