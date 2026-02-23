// ============================================================
//  ANATOMY OF A MICROSOFT AGENT FRAMEWORK AGENT
//  This sample walks through every building block of an agent,
//  annotated step-by-step.
// ============================================================

using System.ClientModel;
using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var githubEndpoint = config["GITHUB_ENDPOINT"] ?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
var githubModel    = config["GITHUB_MODEL_ID"] ?? "gpt-4o-mini";
var githubToken    = config["GITHUB_TOKEN"]    ?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");

// ============================================================
// BUILDING BLOCK 1 — IChatClient
// ============================================================
// An IChatClient is the raw connection to the language model.
// It speaks the chat-completion protocol (send messages, get a reply).
// The Microsoft Agent Framework wraps any IChatClient into an AIAgent.
// Here we create one backed by GitHub Models (OpenAI-compatible endpoint).
// ============================================================

var openAIOptions = new OpenAIClientOptions { Endpoint = new Uri(githubEndpoint) };
IChatClient chatClient = new OpenAIClient(new ApiKeyCredential(githubToken), openAIOptions)
    .GetChatClient(githubModel)
    .AsIChatClient();   // converts OpenAI ChatClient → Microsoft.Extensions.AI IChatClient

// ============================================================
// BUILDING BLOCK 2 — AIAgent
// ============================================================
// AIAgent is the core abstraction in the Microsoft Agent Framework.
// It adds on top of IChatClient:
//   • A name   — identifies the agent in logs and multi-agent workflows
//   • Instructions — the system prompt, shapes the agent's behavior
//   • Tools    — .NET functions the model can call autonomously
// ============================================================

AIAgent agent = chatClient.AsAIAgent(
    name: "FactBot",
    instructions: """
        You are a concise fact assistant that answers questions about world capitals
        and major landmarks. Reply in 2-3 sentences maximum. If asked for a
        random city, call the GetRandomCity tool first.
        """
);

Console.WriteLine("=== BUILDING BLOCK 2: Basic AIAgent ===");
Console.WriteLine("Agent created. Name: FactBot");
Console.WriteLine();

// ============================================================
// EXECUTION PATTERN 1 — RunAsync (single-turn)
// ============================================================
// RunAsync sends a single user message and returns the complete
// response as a string. No conversation state is kept.
// ============================================================

Console.WriteLine("=== EXECUTION PATTERN 1: RunAsync (single-turn) ===");
var response = await agent.RunAsync("What is the capital of Japan, and what is it known for?");
Console.WriteLine(response);
Console.WriteLine();

// ============================================================
// EXECUTION PATTERN 2 — RunStreamingAsync (single-turn, streaming)
// ============================================================
// RunStreamingAsync yields text tokens as they are produced by
// the model. Use this when you want to display output progressively,
// e.g., in a UI or CLI that should feel responsive.
// ============================================================

Console.WriteLine("=== EXECUTION PATTERN 2: RunStreamingAsync (streaming) ===");
await foreach (var token in agent.RunStreamingAsync("What is the Eiffel Tower?"))
{
    Console.Write(token);
}
Console.WriteLine();
Console.WriteLine();

// ============================================================
// BUILDING BLOCK 3 — Tools
// ============================================================
// Tools are ordinary .NET methods the agent can call automatically.
// Two conventions make a method a tool:
//   1. [Description("...")] attribute — passed to the model as the
//      tool's description so it knows when and how to call it.
//   2. AIFunctionFactory.Create(func) — wraps the method into an
//      AIFunction that the agent framework can register and invoke.
// The model decides when to call a tool; the framework handles the
// actual invocation and sends the result back to the model.
// ============================================================

[Description("Returns a random world capital city to use as a topic.")]
static string GetRandomCity()
{
    string[] cities = ["Tokyo", "Paris", "Cairo", "Sydney", "Rio de Janeiro", "Toronto", "Berlin"];
    return cities[Random.Shared.Next(cities.Length)];
}

AIAgent agentWithTools = chatClient.AsAIAgent(
    name: "FactBotWithTools",
    instructions: """
        You are a concise fact assistant. When asked for a random city or topic,
        call GetRandomCity first, then provide 2-3 interesting facts about it.
        """,
    tools: [AIFunctionFactory.Create((Func<string>)GetRandomCity)]
);

Console.WriteLine("=== BUILDING BLOCK 3: Agent with Tools ===");
var toolResponse = await agentWithTools.RunAsync("Give me facts about a random city.");
Console.WriteLine(toolResponse);
Console.WriteLine();

// ============================================================
// BUILDING BLOCK 4 — AgentSession (multi-turn conversation)
// ============================================================
// An AgentSession holds the conversation history for one thread.
// Passing the same session to successive RunAsync calls means the
// model sees all prior messages, enabling follow-up questions and
// context-aware replies.
//
// Without a session, each RunAsync is stateless — the model has
// no memory of previous messages.
// ============================================================

Console.WriteLine("=== BUILDING BLOCK 4: AgentSession (multi-turn) ===");

AgentSession session = await agentWithTools.CreateSessionAsync();

var turn1 = await agentWithTools.RunAsync("Tell me about a random city.", session);
Console.WriteLine($"Turn 1: {turn1}");
Console.WriteLine();

// The agent remembers which city it just mentioned — no need to repeat it.
var turn2 = await agentWithTools.RunAsync("What is the local cuisine like there?", session);
Console.WriteLine($"Turn 2: {turn2}");
Console.WriteLine();

var turn3 = await agentWithTools.RunAsync("And what time zone is it in?", session);
Console.WriteLine($"Turn 3: {turn3}");
Console.WriteLine();

// ============================================================
// SUMMARY
// ============================================================
// The four building blocks compose a full agent:
//
//   IChatClient      — connection to the model
//       ↓
//   AIAgent          — adds name, instructions, tools
//       ↓
//   Tools            — .NET functions the model can call
//       ↓
//   AgentSession     — maintains conversation history (multi-turn)
//
// From here, the next step is to explore multiple agents working
// together in a Workflow (see 07.Workflow) or different providers
// (see 03.ExploreAgentFramework).
// ============================================================

Console.WriteLine("=== Summary ===");
Console.WriteLine("The four building blocks: IChatClient → AIAgent → Tools → AgentSession");
