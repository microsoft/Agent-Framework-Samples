// File-based run: dotnet run app.cs
#:property UserSecretsId 06e4bd3a-9846-4330-98e5-7e0431118217
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
