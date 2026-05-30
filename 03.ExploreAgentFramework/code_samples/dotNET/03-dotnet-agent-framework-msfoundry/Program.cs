using System.ClientModel;
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;
using DotNetEnv;


Env.Load("../../../../.env");

var endpoint = Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT") ?? throw new InvalidOperationException("FOUNDRY_PROJECT_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("FOUNDRY_MODEL") ?? "gpt-4o-mini";

// Create an AI Project client and get an OpenAI client that works with the foundry service.
AIProjectClient aiProjectClient = new(
    new Uri(endpoint),
    new AzureCliCredential());


ProjectsAgentVersion agentVersion = await aiProjectClient.AgentAdministrationClient.CreateAgentVersionAsync(
    "Agent-Framework",
    new ProjectsAgentVersionCreationOptions(
        new DeclarativeAgentDefinition(model: deploymentName)
        {
            Instructions = "You are good at telling jokes."
        }));

#pragma warning disable OPENAI001
FoundryAgent agent = aiProjectClient.AsAIAgent(agentVersion);
#pragma warning restore OPENAI001

Console.WriteLine(await agent.RunAsync("Write a haiku about Agent Framework"));

// You can also create another AIAgent version by providing the same name with a different definition/instruction.
// AIAgent agent = aiProjectClient.CreateAIAgent(name: JokerName, model: deploymentName, instructions: "You are an AI assistant that helps people find information.");

// // You can also get the AIAgent latest version by just providing its name.
// AIAgent jokerAgentLatest = aiProjectClient.GetAIAgent(name: JokerName);
// AgentVersion latestAgentVersion = jokerAgentLatest.GetService<AgentVersion>()!;

// // The AIAgent version can be accessed via the GetService method.
// Console.WriteLine($"Latest agent version id: {latestAgentVersion.Id}");

// // Once you have the AIAgent, you can invoke it like any other AIAgent.
// Console.WriteLine(await jokerAgentLatest.RunAsync("Tell me a joke about a pirate."));

