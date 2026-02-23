// File-based run: dotnet run app.cs
#:property UserSecretsId ee9707f4-4391-43ca-bdd5-38f721eb32a9
#:property EnablePreviewFeatures true
#:package Azure.AI.OpenAI@2.1.0
#:package Azure.Identity@1.17.1
#:package Azure.AI.Projects@1.2.0-beta.5
#:package Azure.AI.Projects.OpenAI@1.0.0-beta.5
#:package Azure.AI.Agents.Persistent@1.2.0-beta.8
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Agents.AI@1.0.0-rc1
#:package Microsoft.Agents.AI.AzureAI@1.0.0-rc1
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0

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
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var azure_foundry_endpoint = config["AZURE_AI_PROJECT_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var azure_foundry_model_id = "gpt-4.1-mini";

const string AgentName = "Bing-Agent-Framework";
const string AgentInstructions = @"You are a helpful assistant that can search the web for current information.
            Use the Bing search tool to find up-to-date information and provide accurate, well-sourced answers.
            Always cite your sources when possible.";

AIProjectClient aiProjectClient = new(
    new Uri(azure_foundry_endpoint),
    new AzureCliCredential());


var connectionName = config["BING_CONNECTION_NAME"];

Console.WriteLine($"Using Bing Connection: {connectionName}");

AIProjectConnection bingConnectionName = aiProjectClient.Connections.GetConnection(connectionName: connectionName);

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
