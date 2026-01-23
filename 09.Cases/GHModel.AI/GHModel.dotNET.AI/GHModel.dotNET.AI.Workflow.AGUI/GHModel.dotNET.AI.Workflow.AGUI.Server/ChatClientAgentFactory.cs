
using System.ClientModel;
using System.ComponentModel;
using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;


using Microsoft.Agents.AI;  
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using Microsoft.AspNetCore.HttpLogging;
using OpenAI;
using DotNetEnv;


internal static class ChatClientAgentFactory
{
    private static OpenAIClient? _openAIClient;
    private static string? github_model_id;

    public static void Initialize()
    {

        Env.Load("../../../../../.env");
        string github_endpoint = Environment.GetEnvironmentVariable("GITHUB_ENDPOINT") ?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
        github_model_id =  Environment.GetEnvironmentVariable("GITHUB_MODEL_ID") ?? throw new InvalidOperationException("GITHUB_MODEL_ID is not set.");
        string github_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");


        var openAIOptions = new OpenAIClientOptions()
        {
            Endpoint = new Uri(github_endpoint)
        };
                
        _openAIClient = new OpenAIClient(new ApiKeyCredential(github_token), openAIOptions);
    }
    public static AIAgent CreateTravelAgenticChat()
    {
        var chatClient = _openAIClient!.GetChatClient(github_model_id!).AsIChatClient();


        string ReviewerAgentName = "Concierge";
        string ReviewerAgentInstructions = @"
            You are an are hotel concierge who has opinions about providing the most local and authentic experiences for travelers.
            The goal is to determine if the front desk travel agent has recommended the best non-touristy experience for a traveler.
            If so, state that it is approved.
            If not, provide insight on how to refine the recommendation without using a specific example. ";

        string FrontDeskAgentName = "FrontDesk";
        string FrontDeskAgentInstructions = @"""
            You are a Front Desk Travel Agent with ten years of experience and are known for brevity as you deal with many customers.
            The goal is to provide the best activities and locations for a traveler to visit.
            Only provide a single recommendation per response.
            You're laser focused on the goal at hand.
            Don't waste time with chit chat.
            Consider suggestions when refining an idea.
            """;

        

        AIAgent reviewerAgent = chatClient.AsAIAgent(
            name:ReviewerAgentName,instructions:ReviewerAgentInstructions);
        AIAgent frontDeskAgent  = chatClient.AsAIAgent(
            name:FrontDeskAgentName,instructions:FrontDeskAgentInstructions);

        var workflow = new WorkflowBuilder(frontDeskAgent)
                    .AddEdge(frontDeskAgent, reviewerAgent)
                    .Build();


        AIAgent workflow_agent = workflow.AsAgent("travel-workflow","travel recommendation workflow");

        return workflow_agent;
    }
    // public static AIAgent CreateSharedState(JsonSerializerOptions options)
    // {
    //     var chatClient = _openAIClient!.GetChatClient(s_deploymentName!).AsIChatClient();

    //     var baseAgent = chatClient.CreateAIAgent(
    //         name: "SharedStateAgent",
    //         description: "An agent that demonstrates shared state patterns using Azure OpenAI");

    //     return new SharedStateAgent(baseAgent, options);
    // }`
}