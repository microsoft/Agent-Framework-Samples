// File-based run: dotnet run app.cs
#:property UserSecretsId 2c130ad6-5dba-4207-af2a-4db898f9e666
#:package Azure.AI.OpenAI@2.1.0
#:package Azure.Identity@1.17.1
#:package Azure.Monitor.OpenTelemetry.Exporter@1.4.0
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Logging.Console@10.0.0
#:package OpenAI@2.8.0
#:package OpenTelemetry@1.13.1
#:package OpenTelemetry.Exporter.Console@1.13.1
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.13.1
#:package OpenTelemetry.Instrumentation.Http@1.13.0
#:package OpenTelemetry.Instrumentation.Runtime@1.13.0
#:package OpenTelemetry.Extensions.Hosting@1.13.1
#:package Microsoft.Agents.AI@1.0.0-rc1
#:package Microsoft.Agents.AI.OpenAI@1.0.0-rc1
#:package Microsoft.Agents.AI.Workflows@1.0.0-rc1
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0

using System;
using System.ComponentModel;
using System.ClientModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenAI;
using Microsoft.Extensions.Configuration;


#region Setup Telemetry

const string SourceName = "OpenTelemetryAspire.ConsoleApp";
const string ServiceName = "WorkflowOpenTelemetry";

// Configure OpenTelemetry for Aspire dashboard
var otlpEndpoint = "http://localhost:4317";

// Create a resource to identify this service
var resource = ResourceBuilder.CreateDefault()
    .AddService(ServiceName, serviceVersion: "1.0.0")
    .AddAttributes(new Dictionary<string, object>
    {
        ["service.instance.id"] = Environment.MachineName,
        ["deployment.environment"] = "development"
    })
    .Build();

// Setup tracing with resource
var tracerProviderBuilder = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName, serviceVersion: "1.0.0"))
    .AddSource(SourceName) // Our custom activity source
    .AddSource("*Microsoft.Agents.AI") // Agent Framework telemetry
    .AddHttpClientInstrumentation() // Capture HTTP calls to OpenAI
    .AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));

using var tracerProvider = tracerProviderBuilder.Build();

// Setup metrics with resource and instrument name filtering
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName, serviceVersion: "1.0.0"))
    .AddMeter(SourceName) // Our custom meter
    .AddMeter("*Microsoft.Agents.AI") // Agent Framework metrics
    .AddHttpClientInstrumentation() // HTTP client metrics
    .AddRuntimeInstrumentation() // .NET runtime metrics
    .AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint))
    .Build();

// Setup structured logging with OpenTelemetry
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Debug)
    .AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName, serviceVersion: "1.0.0"));
        options.AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(otlpEndpoint));
        options.IncludeScopes = true;
        options.IncludeFormattedMessage = true;
    }));

using var activitySource = new ActivitySource(SourceName);
using var meter = new Meter(SourceName);

// Create custom metrics
var interactionCounter = meter.CreateCounter<int>("agent_interactions_total", description: "Total number of agent interactions");
var responseTimeHistogram = meter.CreateHistogram<double>("agent_response_time_seconds", description: "Agent response time in seconds");

#endregion

var serviceProvider = serviceCollection.BuildServiceProvider();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
var appLogger = loggerFactory.CreateLogger<Program>();

Console.WriteLine("""
    === OpenTelemetry Aspire Demo ===
    This demo shows OpenTelemetry integration with the Agent Framework.
    You can view the telemetry data in the Aspire Dashboard.
    Type your message and press Enter. Type 'exit' or empty message to quit.
    """);

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var github_endpoint = config["GITHUB_ENDPOINT"] ?? throw new InvalidOperationException("GITHUB_ENDPOINT is not set.");
var github_model_id =  config["GITHUB_MODEL_ID"] ?? throw new InvalidOperationException("GITHUB_MODEL_ID is not set.");
var github_token = config["GITHUB_TOKEN"] ?? throw new InvalidOperationException("GITHUB_TOKEN is not set.");

var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri(github_endpoint)
};
        
var openAIClient = new OpenAIClient(new ApiKeyCredential(github_token), openAIOptions);



var chatClient = openAIClient.GetChatClient(github_model_id).AsIChatClient();

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



var reviewerAgentBuilder = new AIAgentBuilder(chatClient.AsAIAgent(
    name: ReviewerAgentName,
    instructions: ReviewerAgentInstructions))
    .UseOpenTelemetry(SourceName, configure: cfg => cfg.EnableSensitiveData = true);

var frontDeskAgentBuilder = new AIAgentBuilder(chatClient.AsAIAgent(
    name: FrontDeskAgentName,
    instructions: FrontDeskAgentInstructions))
    .UseOpenTelemetry(SourceName, configure: cfg => cfg.EnableSensitiveData = true);

AIAgent reviewerAgent = reviewerAgentBuilder.Build(serviceProvider);
AIAgent frontDeskAgent = frontDeskAgentBuilder.Build(serviceProvider);

var workflow = new WorkflowBuilder(frontDeskAgent)
            .AddEdge(frontDeskAgent, reviewerAgent)
            .Build();


var workflowAgentBuilder = new AIAgentBuilder(workflow.AsAIAgent(id: "travel-workflow", name: "travel-workflow", description: "travel recommendation workflow"))
    .UseOpenTelemetry(SourceName, configure: cfg => cfg.EnableSensitiveData = true);

AIAgent workflow_agent = workflowAgentBuilder.Build(serviceProvider);

AgentSession session = await workflow_agent.CreateSessionAsync();

Console.WriteLine("\n💡 Before starting, run the VS Code command 'AI Toolkit: Open Trace Viewer (ai-mlstudio.tracing.open)' to capture traces in the AI Toolkit collector.\n");

while (true)
{
    Console.Write("Traveler: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput) || string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
    {
        appLogger.LogInformation("Conversation ended by user request.");
        break;
    }

    interactionCounter.Add(1);

    using var interactionActivity = activitySource.StartActivity("workflow.interaction", ActivityKind.Client);

    interactionActivity?.SetTag("workflow.agent.id", workflow_agent.Id);
    interactionActivity?.SetTag("workflow.agent.name", workflow_agent.Name);
    interactionActivity?.SetTag("prompt.length", userInput.Length);

    var stopwatch = Stopwatch.StartNew();

    try
    {
        var response = await workflow_agent.RunAsync(
            new[] { new ChatMessage(ChatRole.User, userInput) },
            session,
            cancellationToken: CancellationToken.None);

        stopwatch.Stop();

        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        responseTimeHistogram.Record(elapsedSeconds);

        interactionActivity?.SetTag("response.elapsed.seconds", elapsedSeconds);
        interactionActivity?.SetTag("response.message.count", response.Messages.Count);

        var text = response.Text;
        interactionActivity?.SetTag("response.length", text.Length);
        interactionActivity?.SetStatus(ActivityStatusCode.Ok);

        Console.WriteLine($"\nWorkflow: {text}");
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        responseTimeHistogram.Record(elapsedSeconds);

        interactionActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        interactionActivity?.AddEvent(new ActivityEvent(
            "exception",
            tags: new ActivityTagsCollection
            {
                { "exception.type", ex.GetType().FullName ?? string.Empty },
                { "exception.message", ex.Message }
            }));

        appLogger.LogError(ex, "Workflow interaction failed.");
        Console.WriteLine("⚠️ Workflow interaction failed. Check logs for details.");
    }
}
