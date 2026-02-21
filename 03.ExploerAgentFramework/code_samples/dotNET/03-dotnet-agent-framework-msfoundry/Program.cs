using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var endpoint = config["AZURE_AI_PROJECT_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var deploymentName = config["AZURE_AI_MODEL_DEPLOYMENT_NAME"] ?? "gpt-4o-mini";

// Create an AI Project client and get an OpenAI client that works with the foundry service.
AIProjectClient aiProjectClient = new(
    new Uri(endpoint),
    new AzureCliCredential());


AIAgent agent = await aiProjectClient.CreateAIAgentAsync(
    name: "Agent-Framework",
    creationOptions: new AgentVersionCreationOptions(
        new PromptAgentDefinition(model: deploymentName)
        {
            Instructions = "You are good at telling jokes."
        })
);


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

