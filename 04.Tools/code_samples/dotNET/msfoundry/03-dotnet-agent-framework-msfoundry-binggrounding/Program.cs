using System;
using System.Linq;
using System.IO;
using System.Text;
using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Assistants;
using OpenAI.Responses;
using DotNetEnv;

// using OpenAI.Assistants;
// using OpenAI.Responses;


 Env.Load("../../../../../.env");


var azure_foundry_endpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT") ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var azure_foundry_model_id = "gpt-4.1-mini";

const string AgentName = "Bing-Agent-Framework";
const string AgentInstructions = @"You are a helpful assistant that can search the web for current information.
            Use the Bing search tool to find up-to-date information and provide accurate, well-sourced answers.
            Always cite your sources when possible.";

AIProjectClient aiProjectClient = new(
    new Uri(azure_foundry_endpoint),
    new AzureCliCredential());


var connectionName = Environment.GetEnvironmentVariable("BING_CONNECTION_NAME");

Console.WriteLine($"Using Bing Connection: {connectionName}");

AIProjectConnection bingConnectionName = aiProjectClient.Connections.GetConnection(connectionName: connectionName);


// AIProjectConnection bingConnectionName = projectClient.Connections.GetConnection(connectionName: connectionName);

BingGroundingAgentTool bingGroundingAgentTool = new(new BingGroundingSearchToolOptions(
    searchConfigurations: [new BingGroundingSearchConfiguration(projectConnectionId: bingConnectionName.Id)]
    )
);


AIAgent bingAgent = await aiProjectClient.CreateAIAgentAsync(
    name: AgentName,
    creationOptions: new AgentVersionCreationOptions(
        new PromptAgentDefinition(model: azure_foundry_model_id)
        {
            Instructions = AgentInstructions,
            Tools = {
                    bingGroundingAgentTool,
            }
        })
);

AgentResponse response = await bingAgent.RunAsync("What is today's date and weather in Guangzhou?");

Console.WriteLine("Response:");
Console.WriteLine(response);
