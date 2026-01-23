
// using System.ComponentModel;
// using Azure.AI.OpenAI;
// using Azure.Identity;
// using Microsoft.Agents.AI;
// using Microsoft.Agents.AI.DevUI;
// using Microsoft.Agents.AI.Hosting;
// using Microsoft.Agents.AI.Workflows;
// using Microsoft.Extensions.AI;

using System;
using System.ComponentModel;
using System.ClientModel;
using Azure.Identity;   
using DotNetEnv;
using OpenAI;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;


namespace GHModel.dotNET.AI.Workflow.DevUI;

/// <summary>
/// Sample demonstrating basic usage of the DevUI in an ASP.NET Core application.
/// </summary>
/// <remarks>
/// This sample shows how to:
/// 1. Set up Azure OpenAI as the chat client
/// 2. Create function tools for agents to use
/// 3. Register agents and workflows using the hosting packages with tools
/// 4. Map the DevUI endpoint which automatically configures the middleware
/// 5. Map the dynamic OpenAI Responses API for Python DevUI compatibility
/// 6. Access the DevUI in a web browser
///
/// The DevUI provides an interactive web interface for testing and debugging AI agents.
/// DevUI assets are served from embedded resources within the assembly.
/// Simply call MapDevUI() to set up everything needed.
///
/// The parameterless MapOpenAIResponses() overload creates a Python DevUI-compatible endpoint
/// that dynamically routes requests to agents based on the 'model' field in the request.
/// </remarks>
internal static class Program
{
    /// <summary>
    /// Entry point that starts an ASP.NET Core web server with the DevUI.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    private static void Main(string[] args)
    {
        // Load environment variables from .env file
        Env.Load("../../../../.env");

        var builder = WebApplication.CreateBuilder(args);

        // Set up the Azure OpenAI client
        var github_endpoint = Environment.GetEnvironmentVariable("GITHUB_ENDPOINT") ?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
        var github_model_id =  "openai/gpt-5";
        var github_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");

        var openAIOptions = new OpenAIClientOptions()
        {
            Endpoint = new Uri(github_endpoint)
        };
        
        var openAIClient = new OpenAIClient(new ApiKeyCredential(github_token), openAIOptions);

        var chatClient = openAIClient.GetChatClient(github_model_id).AsIChatClient();


        builder.Services.AddChatClient(chatClient);

        // Register sample agents with tools
        builder.AddAIAgent("GPTAssistant", "You are a helpful assistant. Answer questions concisely and accurately.");

        const string ReviewerAgentName = "Concierge";
        const string ReviewerAgentInstructions = @"
            You are an are hotel concierge who has opinions about providing the most local and authentic experiences for travelers.
            The goal is to determine if the front desk travel agent has recommended the best non-touristy experience for a traveler.
            If so, state that it is approved.
            If not, provide insight on how to refine the recommendation without using a specific example. ";

        const string FrontDeskAgentName = "FrontDesk";
        const string FrontDeskAgentInstructions = @"""
            You are a Front Desk Travel Agent with ten years of experience and are known for brevity as you deal with many customers.
            The goal is to provide the best activities and locations for a traveler to visit.
            Only provide a single recommendation per response.
            You're laser focused on the goal at hand.
            Don't waste time with chit chat.
            Consider suggestions when refining an idea.
            """;



        var reviewerAgentBuilder = builder.AddAIAgent(ReviewerAgentName, ReviewerAgentInstructions);
        var frontDeskAgentBuilder = builder.AddAIAgent(FrontDeskAgentName, FrontDeskAgentInstructions);

        builder.AddWorkflow("gh-model-workflow", (sp, key) =>
        {
            var agents = new List<IHostedAgentBuilder>() { reviewerAgentBuilder, frontDeskAgentBuilder }.Select(ab => sp.GetRequiredKeyedService<AIAgent>(ab.Name));
            return AgentWorkflowBuilder.BuildSequential(workflowName: key, agents: agents);
        }).AddAsAIAgent();

        builder.Services.AddOpenAIResponses();
        builder.Services.AddOpenAIConversations();

        var app = builder.Build();


        app.UseHttpsRedirection();

        app.MapOpenAIResponses();
        app.MapOpenAIConversations();

        if (builder.Environment.IsDevelopment())
        {
            app.MapDevUI();
        }

        Console.WriteLine("DevUI is available at: http://localhost:50518/devui");
        Console.WriteLine("OpenAI Responses API is available at: http://localhost:50518/v1/responses");
        Console.WriteLine("Press Ctrl+C to stop the server.");
        app.Run();
    }
}
