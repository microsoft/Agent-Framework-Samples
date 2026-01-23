
using System;
using System.ComponentModel;
using System.ClientModel;
using AGUIDojoServer;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;  
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using Microsoft.AspNetCore.HttpLogging;
using OpenAI;
using OpenAI.Chat;
using DotNetEnv;

Env.Load("../../../../../.env");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.RequestBody
        | HttpLoggingFields.ResponsePropertiesAndHeaders | HttpLoggingFields.ResponseBody;
    logging.RequestBodyLogLimit = int.MaxValue;
    logging.ResponseBodyLogLimit = int.MaxValue;
});

builder.Services.AddHttpClient().AddLogging();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Add(AGUIDojoServerSerializerContext.Default));
// builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Add(AGUIDojoServerSerializerContext.Default));
builder.Services.AddAGUI();


ChatClientAgentFactory.Initialize();

WebApplication app = builder.Build();

// var github_endpoint = Environment.GetEnvironmentVariable("GITHUB_ENDPOINT") ?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
// var github_model_id =  Environment.GetEnvironmentVariable("GITHUB_MODEL_ID") ?? throw new InvalidOperationException("GITHUB_MODEL_ID is not set.");
// var github_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");

// var openAIOptions = new OpenAIClientOptions()
// {
//     Endpoint = new Uri(github_endpoint)
// };
        
// var openAIClient = new OpenAIClient(new ApiKeyCredential(github_token), openAIOptions);



// var chatClient = openAIClient.GetChatClient(github_model_id).AsIChatClient();

// const string ReviewerAgentName = "Concierge";
// const string ReviewerAgentInstructions = @"
//     You are an are hotel concierge who has opinions about providing the most local and authentic experiences for travelers.
//     The goal is to determine if the front desk travel agent has recommended the best non-touristy experience for a traveler.
//     If so, state that it is approved.
//     If not, provide insight on how to refine the recommendation without using a specific example. ";

// const string FrontDeskAgentName = "FrontDesk";
// const string FrontDeskAgentInstructions = @"""
//     You are a Front Desk Travel Agent with ten years of experience and are known for brevity as you deal with many customers.
//     The goal is to provide the best activities and locations for a traveler to visit.
//     Only provide a single recommendation per response.
//     You're laser focused on the goal at hand.
//     Don't waste time with chit chat.
//     Consider suggestions when refining an idea.
//     """;



// AIAgent reviewerAgent = chatClient.CreateAIAgent(
//     name:ReviewerAgentName,instructions:ReviewerAgentInstructions);
// AIAgent frontDeskAgent  = chatClient.CreateAIAgent(
//     name:FrontDeskAgentName,instructions:FrontDeskAgentInstructions);

// var workflow = new WorkflowBuilder(frontDeskAgent)
//             .AddEdge(frontDeskAgent, reviewerAgent)
//             .Build();


// AIAgent workflow_agent = workflow.AsAgent("travel-workflow","travel recommendation workflow");


// Map the AG-UI agent endpoint
app.MapAGUI("/", ChatClientAgentFactory.CreateTravelAgenticChat());
await app.RunAsync();


public partial class Program;