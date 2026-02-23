# Tutorial: Creating MCP and A2A Applications in Agent Framework

This tutorial will guide you through the concepts and practical examples of creating applications using the Model Context Protocol (MCP) and Agent-to-Agent (A2A) patterns within the Agent Framework. We will use the provided .NET and Python code samples to illustrate how to connect an agent to an external toolset via MCP.

## MCP (Model Context Protocol)

### Concept of MCP

The Model Context Protocol (MCP) is a standard that allows an AI agent to discover and interact with external tools and services. Think of it as a universal API for agents. Instead of custom-building a connection for every tool, an agent can connect to an MCP endpoint. This endpoint exposes a list of available tools, which the agent can then intelligently use to fulfill user requests.

In the provided examples, the agent connects to the Microsoft Learn MCP endpoint (`https://learn.microsoft.com/api/mcp`), which gives it the ability to search and retrieve information directly from Microsoft's documentation. This is a powerful way to ground the agent with up-to-date, specific information.

### Application Scenarios for MCP

The primary use case for MCP is **Retrieval-Augmented Generation (RAG)**. This pattern enhances an agent's capabilities by allowing it to fetch information from external knowledge bases before generating a response.

Common scenarios include:

  * **Answering questions about documentation:** An agent can provide accurate answers about proprietary software, internal company documents, or rapidly changing product information by querying a documentation server through MCP.
  * **Task automation:** An agent can use MCP-exposed tools to perform actions, such as looking up product inventory, checking a user's order status, or filing a support ticket.
  * **Dynamic tool usage:** As new tools are added to the MCP server, the agent can automatically discover and use them without needing to be reprogrammed.

### MCP Examples

Here are practical examples in both .NET and Python that demonstrate how to create an agent that uses an MCP tool.

#### 1\. .NET Example

This C\# code demonstrates how to create a persistent agent in Azure AI Foundry, equip it with an MCP tool definition, and use it to answer a query.

**`Program.cs`**

```csharp
using ModelContextProtocol.Client;

using System;
using System.Linq;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

using DotNetEnv;

// Load environment variables
Env.Load("./.env");

var azure_foundry_endpoint = Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT") ?? throw new InvalidOperationException("AZURE_FOUNDRY_PROJECT_ENDPOINT is not set.");
var azure_foundry_model_id = Environment.GetEnvironmentVariable("FOUNDRY_MODEL_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

// Connect to the Persistent Agents Client
var persistentAgentsClient = new PersistentAgentsClient(azure_foundry_endpoint, new AzureCliCredential());

// Define the MCP tool endpoint
MCPToolDefinition mcpTool = new("mslearnmcp", "https://learn.microsoft.com/api/mcp");
string searchMSLearn = "searchmslearn";
mcpTool.AllowedTools.Add(searchMSLearn);

// Create the agent definition
var agentModel = await persistentAgentsClient.Administration.CreateAgentAsync(
            model:azure_foundry_model_id,
            name: "MSLearnMCPAgent",
            instructions: "You are a helpful agent that can use MCP tools to assist users. Use the available MCP tools to answer questions and perform tasks.",
            tools: [mcpTool]
            );

// Get the created agent instance
AIAgent agent = await persistentAgentsClient.GetAIAgentAsync(agentModel.Value.Id);

Console.WriteLine($"Created agent with ID: {agent.Id}");

// Create an MCP client to inspect the tools on the server
IMcpClient mcpClient = await McpClientFactory.CreateAsync(
    new SseClientTransport(new SseClientTransportOptions()
    {
        Endpoint = new Uri("https://learn.microsoft.com/api/mcp")
    })
);

// List the available tools from the MCP endpoint
IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

Console.WriteLine("Available tools:");
foreach (var tool in tools)
{
    Console.WriteLine($"  {tool.Name}: {tool.Description}");
}

// Create a new conversation thread
AgentThread thread = agent.GetNewThread();

ChatMessage userMessage = new ChatMessage(ChatRole.User, "What is Foundry Local?");


var chatOptions = new ChatClientAgentRunOptions
{
    ChatOptions = new ChatOptions
    {
        Tools = [.. tools]
    }
};

// Run the agent with the user's query
Console.WriteLine(await agent.RunAsync("What's Foundry Local?", thread, chatOptions));
```

#### 2\. Python Example

This Python example, designed for a Jupyter Notebook, shows two ways to provide the MCP tool to an agent: either at runtime or during its creation.

**`01-python-agent-framework-aifoundry-mcp.ipynb`**

**Cell 1: Imports**

```python
from agent_framework import ChatAgent, MCPStreamableHTTPTool
from agent_framework.azure import AzureAIAgentClient
from azure.identity.aio import AzureCliCredential
```

**Cell 2: Method 1 - Pass the Tool During the `run` Call**
In this approach, the MCP tool is created as a context manager and passed to the agent when the `run` method is invoked. This is useful for temporary or session-based tool usage.

```python
async with (
        AzureCliCredential() as credential,
        MCPStreamableHTTPTool(
            name="Microsoft Learn MCP",
            url="https://learn.microsoft.com/api/mcp",
        ) as mcp_server,
        ChatAgent(
            chat_client=AzureAIAgentClient(async_credential=credential),
            name="DocsAgent",
            instructions="You are a helpful assistant that can help with microsoft documentation questions.",
        ) as agent,
):
        query = "What is Microsoft Semantic Kernel?"
        print(f"User: {query}")
        result = await agent.run(query, tools=mcp_server)
        print(f"{agent.name}: {result}\n")
```

**Cell 3: Method 2 - Define the Tool at Agent Creation**
Here, the `MCPStreamableHTTPTool` is passed directly into the `create_agent` method. This makes the tool a permanent part of the agent's definition, available for all subsequent calls without needing to be passed in again.

```python
async with (
        AzureCliCredential() as credential,
        AzureAIAgentClient(async_credential=credential).create_agent(
            name="DocsAgent",
            instructions="You are a helpful assistant that can help with microsoft documentation questions.",
            tools=MCPStreamableHTTPTool(  # Tool is defined here
                name="Microsoft Learn MCP",
                url="https://learn.microsoft.com/api/mcp",
            ),
        ) as agent,
):
        query = "What is Microsoft Semantic Kernel?"
        print(f"User: {query}")
        result = await agent.run(query)
        print(f"{agent.name}: {result}\n")
```

## A2A (Agent-to-Agent)

While the provided code files focus on the MCP pattern, the A2A pattern is another core concept in building sophisticated multi-agent systems.

### Concept of A2A

Agent-to-Agent (A2A) communication is a pattern where one AI agent can call upon another AI agent to perform a task or answer a question. This allows developers to build complex systems by composing smaller, specialized agents. Each agent can have its own unique instructions, capabilities, and tools.

For example, a "Travel Planner" agent could delegate tasks by calling a "Flight Booker" agent and a "Hotel Reservation" agent. The Travel Planner orchestrates the overall goal, while the specialized agents handle their specific domains.

### Application Scenarios for A2A

  * **Task Delegation and Orchestration:** A primary "manager" agent can break down a complex user request into sub-tasks and assign them to different "worker" agents.
  * **Specialized Expertise:** A generalist agent can consult a specialist agent for deep knowledge in a specific area (e.g., a "General Support" agent calling a "Billing Expert" agent).

  * **Collaborative Problem-Solving:** Multiple agents can work together, sharing information and intermediate results to solve a problem that would be too complex for a single agent.

### A2A Example

#### .NET Example

This sample demonstrates the A2A pattern using GitHub Models. A **ResearchAgent** specialist is wrapped as a callable tool for an **OrchestratorAgent**, which delegates research queries to it at runtime.

The key mechanism is `AIFunctionFactory.Create`, which turns any .NET delegate — including another agent's `RunAsync` — into an `AIFunction` the model can invoke as a tool call.

```csharp
// Wrap ResearchAgent as a callable tool for another agent
Func<string, Task<string>> researchFunc =
    async (query) => (await researchAgent.RunAsync(query)).ToString();

var researchTool = AIFunctionFactory.Create(
    (Func<string, Task<string>>)researchFunc,
    name: "ResearchTopic",
    description: "Delegates a research query to the ResearchAgent specialist.");

// OrchestratorAgent treats ResearchAgent as a provider
AIAgent orchestratorAgent = chatClient.AsAIAgent(
    name: "OrchestratorAgent",
    instructions: "For every question, call ResearchTopic first, then compose a friendly answer.",
    tools: [researchTool]);

var answer = await orchestratorAgent.RunAsync("What should I know about visiting Tokyo?");
Console.WriteLine(answer);
```

Full sample: [02-dotnet-agent-framework-ghmodel-a2a](./code_samples/dotNET/02-dotnet-agent-framework-ghmodel-a2a/)

**Setup**

```bash
cd 05.Providers/code_samples/dotNET/02-dotnet-agent-framework-ghmodel-a2a

dotnet user-secrets set "GITHUB_TOKEN"    "<your-github-token>"
dotnet user-secrets set "GITHUB_ENDPOINT" "https://models.inference.ai.azure.com"
dotnet user-secrets set "GITHUB_MODEL_ID" "gpt-4o-mini"
```

**Run**

```bash
dotnet run app.cs   # file-based
# or
dotnet run          # project-based
```

> **Compare with 07.Workflow:** In a Workflow, agents are wired into a fixed pipeline and messages pass through in sequence. In A2A, the orchestrator decides at runtime whether and when to call the specialist — it is a dynamic tool call, not a predetermined edge in a graph.
