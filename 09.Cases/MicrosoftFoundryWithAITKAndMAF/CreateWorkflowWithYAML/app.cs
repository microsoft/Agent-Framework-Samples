// File-based run: dotnet run app.cs
// Run from the CreateWorkflowWithYAML/ directory: dotnet run app.cs
#:property UserSecretsId 9be133a2-f120-490b-bf73-6cfa7a742fce
#:package Microsoft.Agents.AI.Workflows@1.0.0-rc1
#:package Microsoft.Agents.AI.Workflows.Declarative@1.0.0-rc1
#:package Microsoft.Agents.AI.Workflows.Declarative.AzureAI@1.0.0-rc1
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.Extensions.Configuration.Binder@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0
#:package Microsoft.Extensions.Configuration.Json@10.0.0
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.ClientModel@1.8.1
#:package System.Collections.Immutable@10.0.0
#:package Azure.Identity@1.17.1

// Types are for evaluation purposes only and is subject to change or removal in future updates.
#pragma warning disable OPENAI001
#pragma warning disable OPENAICUA001
#pragma warning disable MEAI001

using Azure.Identity;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Checkpointing;
using Microsoft.Agents.AI.Workflows.Declarative;
using Microsoft.Agents.AI.Workflows.Declarative.Events;
using Microsoft.Agents.AI.Workflows.Declarative.Kit;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenAI.Responses;
using System.Diagnostics;
using System.Text.Json;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

Uri foundryEndpoint = new(config["AZURE_AI_PROJECT_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set. Run: dotnet user-secrets set \"AZURE_AI_PROJECT_ENDPOINT\" \"<value>\""));
// For file-based run, YAML files are at ../YAML/ relative to CreateWorkflowWithYAML/
string yamlPath = config["WORKFLOW_YAML_PATH"] ?? "../YAML/workflow.yaml";

WorkflowFactory workflowFactory = new(yamlPath, foundryEndpoint);
WorkflowRunner runner = new();
await runner.ExecuteAsync(workflowFactory.CreateWorkflow, "junior developer");

// WorkflowFactory inlined from WorkflowFactory.cs

internal sealed class WorkflowFactory(string workflowFile, Uri foundryEndpoint)
{
    public IList<AIFunction> Functions { get; init; } = [];

    public IConfiguration? Configuration { get; init; }

    // Assign to continue an existing conversation
    public string? ConversationId { get; init; }

    // Assign to enable logging
    public ILoggerFactory LoggerFactory { get; init; } = NullLoggerFactory.Instance;

    /// <summary>
    /// Create the workflow from the declarative YAML.  Includes definition of the
    /// <see cref="DeclarativeWorkflowOptions" /> and the associated <see cref="WorkflowAgentProvider"/>.
    /// </summary>
    public Workflow CreateWorkflow()
    {
        // Create the agent provider that will service agent requests within the workflow.
        AzureAgentProvider agentProvider = new(foundryEndpoint, new AzureCliCredential())
        {
            // Functions included here will be auto-executed by the framework.
            Functions = this.Functions
        };

        // Define the workflow options.
        DeclarativeWorkflowOptions options =
            new(agentProvider)
            {
                Configuration = this.Configuration,
                ConversationId = this.ConversationId,
                LoggerFactory = this.LoggerFactory,
            };

        // Use Directory.GetCurrentDirectory() so file-based run resolves paths from CWD
        string workflowPath = Path.Combine(Directory.GetCurrentDirectory(), workflowFile);

        // Use DeclarativeWorkflowBuilder to build a workflow based on a YAML file.
        return DeclarativeWorkflowBuilder.Build<string>(workflowPath, options);
    }
}

// WorkflowRunner inlined from WorkflowRunner.cs

internal sealed class WorkflowRunner
{
    private Dictionary<string, AIFunction> FunctionMap { get; }
    private CheckpointInfo? LastCheckpoint { get; set; }

    public static void Notify(string message, ConsoleColor? color = null)
    {
        Console.ForegroundColor = color ?? ConsoleColor.Cyan;
        try
        {
            Console.WriteLine(message);
        }
        finally
        {
            Console.ResetColor();
        }
    }

    /// <summary>
    /// When enabled, checkpoints will be persisted to disk as JSON files.
    /// Otherwise  an in-memory checkpoint store that will not persist checkpoints
    /// beyond the lifetime of the process.
    /// </summary>
    public bool UseJsonCheckpoints { get; init; }

    public WorkflowRunner(params IEnumerable<AIFunction> functions)
    {
        this.FunctionMap = functions.ToDictionary(f => f.Name);
    }

    public async Task ExecuteAsync(Func<Workflow> workflowProvider, string input)
    {
        Workflow workflow = workflowProvider.Invoke();

        CheckpointManager checkpointManager;

        if (this.UseJsonCheckpoints)
        {
            // Use a file-system based JSON checkpoint store to persist checkpoints to disk.
            DirectoryInfo checkpointFolder = Directory.CreateDirectory(Path.Combine(".", $"chk-{DateTime.Now:yyMMdd-hhmmss-ff}"));
            checkpointManager = CheckpointManager.CreateJson(new FileSystemJsonCheckpointStore(checkpointFolder));
        }
        else
        {
            // Use an in-memory checkpoint store that will not persist checkpoints beyond the lifetime of the process.
            checkpointManager = CheckpointManager.CreateInMemory();
        }

        StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, input, checkpointManager).ConfigureAwait(false);

        bool isComplete = false;
        ExternalResponse? requestResponse = null;
        do
        {
            ExternalRequest? externalRequest = await this.MonitorAndDisposeWorkflowRunAsync(run, requestResponse).ConfigureAwait(false);
            if (externalRequest is not null)
            {
                Notify("\nWORKFLOW: Yield\n", ConsoleColor.DarkYellow);

                if (this.LastCheckpoint is null)
                {
                    throw new InvalidOperationException("Checkpoint information missing after external request.");
                }

                // Process the external request.
                object response = await this.HandleExternalRequestAsync(externalRequest).ConfigureAwait(false);
                requestResponse = externalRequest.CreateResponse(response);

                // Let's resume on an entirely new workflow instance to demonstrate checkpoint portability.
                workflow = workflowProvider.Invoke();

                // Restore the latest checkpoint.
                Debug.WriteLine($"RESTORE #{this.LastCheckpoint.CheckpointId}");
                Notify("WORKFLOW: Restore", ConsoleColor.DarkYellow);

                run = await InProcessExecution.ResumeStreamingAsync(workflow, this.LastCheckpoint!, checkpointManager).ConfigureAwait(false);
            }
            else
            {
                isComplete = true;
            }
        }
        while (!isComplete);

        Notify("\nWORKFLOW: Done!\n");
    }

    public async Task<ExternalRequest?> MonitorAndDisposeWorkflowRunAsync(StreamingRun run, ExternalResponse? response = null)
    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using IAsyncDisposable disposeRun = run;
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        bool hasStreamed = false;
        string? messageId = null;

        bool shouldExit = false;
        ExternalRequest? externalResponse = null;

        if (response is not null)
        {
            await run.SendResponseAsync(response).ConfigureAwait(false);
        }

        await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync().ConfigureAwait(false))
        {
            switch (workflowEvent)
            {
                case ExecutorInvokedEvent executorInvoked:
                    Debug.WriteLine($"EXECUTOR ENTER #{executorInvoked.ExecutorId}");
                    break;

                case ExecutorCompletedEvent executorCompleted:
                    Debug.WriteLine($"EXECUTOR EXIT #{executorCompleted.ExecutorId}");
                    break;

                case DeclarativeActionInvokedEvent actionInvoked:
                    Debug.WriteLine($"ACTION ENTER #{actionInvoked.ActionId} [{actionInvoked.ActionType}]");
                    break;

                case DeclarativeActionCompletedEvent actionComplete:
                    Debug.WriteLine($"ACTION EXIT #{actionComplete.ActionId} [{actionComplete.ActionType}]");
                    break;

                case ExecutorFailedEvent executorFailure:
                    Debug.WriteLine($"STEP ERROR #{executorFailure.ExecutorId}: {executorFailure.Data?.Message ?? "Unknown"}");
                    break;

                case WorkflowErrorEvent workflowError:
                    throw workflowError.Data as Exception ?? new InvalidOperationException("Unexpected failure...");

                case SuperStepCompletedEvent checkpointCompleted:
                    this.LastCheckpoint = checkpointCompleted.CompletionInfo?.Checkpoint;
                    Debug.WriteLine($"CHECKPOINT x{checkpointCompleted.StepNumber} [{this.LastCheckpoint?.CheckpointId ?? "(none)"}]");
                    if (externalResponse is not null)
                    {
                        shouldExit = true;
                    }
                    break;

                case RequestInfoEvent requestInfo:
                    Debug.WriteLine($"REQUEST #{requestInfo.Request.RequestId}");
                    externalResponse = requestInfo.Request;
                    break;

                case ConversationUpdateEvent invokeEvent:
                    Debug.WriteLine($"CONVERSATION: {invokeEvent.Data}");
                    break;

                case MessageActivityEvent activityEvent:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nACTIVITY:");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(activityEvent.Message.Trim());
                    break;

                case AgentResponseUpdateEvent streamEvent:
                    if (!string.Equals(messageId, streamEvent.Update.MessageId, StringComparison.Ordinal))
                    {
                        hasStreamed = false;
                        messageId = streamEvent.Update.MessageId;

                        if (messageId is not null)
                        {
                            string? agentName = streamEvent.Update.AuthorName ?? streamEvent.Update.AgentId ?? nameof(ChatRole.Assistant);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write($"\n{agentName.ToUpperInvariant()}:");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($" [{messageId}]");
                        }
                    }

                    ChatResponseUpdate? chatUpdate = streamEvent.Update.RawRepresentation as ChatResponseUpdate;
                    switch (chatUpdate?.RawRepresentation)
                    {
                        case ImageGenerationCallResponseItem messageUpdate:
                            await DownloadFileContentAsync(Path.GetFileName("response.png"), messageUpdate.ImageResultBytes).ConfigureAwait(false);
                            break;

                        case FunctionCallResponseItem actionUpdate:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write($"Calling tool: {actionUpdate.FunctionName}");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($" [{actionUpdate.CallId}]");
                            break;

                        case McpToolCallItem actionUpdate:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write($"Calling tool: {actionUpdate.ToolName}");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($" [{actionUpdate.Id}]");
                            break;
                    }

                    try
                    {
                        Console.ResetColor();
                        Console.Write(streamEvent.Update.Text);
                        hasStreamed |= !string.IsNullOrEmpty(streamEvent.Update.Text);
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                    break;

                case AgentResponseEvent messageEvent:
                    try
                    {
                        if (hasStreamed)
                        {
                            Console.WriteLine();
                        }

                        if (messageEvent.Response.Usage is not null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"[Tokens Total: {messageEvent.Response.Usage.TotalTokenCount}, Input: {messageEvent.Response.Usage.InputTokenCount}, Output: {messageEvent.Response.Usage.OutputTokenCount}]");
                        }
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                    break;

                default:
                    break;
            }

            if (shouldExit)
            {
                break;
            }
        }

        return externalResponse;
    }

    /// <summary>
    /// Handle request for external input.
    /// </summary>
    private async ValueTask<ExternalInputResponse> HandleExternalRequestAsync(ExternalRequest request)
    {
        if (!request.TryGetDataAs<ExternalInputRequest>(out var inputRequest))
        {
            throw new InvalidOperationException($"Expected external request type: {request.PortInfo.RequestType}.");
        }

        List<ChatMessage> responseMessages = [];

        foreach (ChatMessage message in inputRequest.AgentResponse.Messages)
        {
            await foreach (ChatMessage responseMessage in this.ProcessInputMessageAsync(message).ConfigureAwait(false))
            {
                responseMessages.Add(responseMessage);
            }
        }

        if (responseMessages.Count == 0)
        {
            // Must be request for user input.
            responseMessages.Add(HandleUserInputRequest(inputRequest));
        }

        Console.WriteLine();

        return new ExternalInputResponse(responseMessages);
    }

    private async IAsyncEnumerable<ChatMessage> ProcessInputMessageAsync(ChatMessage message)
    {
        foreach (AIContent requestItem in message.Contents)
        {
            ChatMessage? responseMessage =
                requestItem switch
                {
                    FunctionCallContent functionCall => await InvokeFunctionAsync(functionCall).ConfigureAwait(false),
                    FunctionApprovalRequestContent functionApprovalRequest => ApproveFunction(functionApprovalRequest),
                    McpServerToolApprovalRequestContent mcpApprovalRequest => ApproveMCP(mcpApprovalRequest),
                    _ => HandleUnknown(requestItem),
                };

            if (responseMessage is not null)
            {
                yield return responseMessage;
            }
        }

        ChatMessage? HandleUnknown(AIContent request)
        {
            return null;
        }

        ChatMessage ApproveFunction(FunctionApprovalRequestContent functionApprovalRequest)
        {
            Notify($"INPUT - Approving Function: {functionApprovalRequest.FunctionCall.Name}");
            return new ChatMessage(ChatRole.User, [functionApprovalRequest.CreateResponse(approved: true)]);
        }

        ChatMessage ApproveMCP(McpServerToolApprovalRequestContent mcpApprovalRequest)
        {
            Notify($"INPUT - Approving MCP: {mcpApprovalRequest.ToolCall.ToolName}");
            return new ChatMessage(ChatRole.User, [mcpApprovalRequest.CreateResponse(approved: true)]);
        }

        async Task<ChatMessage> InvokeFunctionAsync(FunctionCallContent functionCall)
        {
            Notify($"INPUT - Executing Function: {functionCall.Name}");
            AIFunction functionTool = this.FunctionMap[functionCall.Name];
            AIFunctionArguments? functionArguments = functionCall.Arguments is null ? null : new(functionCall.Arguments.NormalizePortableValues());
            object? result = await functionTool.InvokeAsync(functionArguments).ConfigureAwait(false);
            return new ChatMessage(ChatRole.Tool, [new FunctionResultContent(functionCall.CallId, JsonSerializer.Serialize(result))]);
        }
    }

    private static ChatMessage HandleUserInputRequest(ExternalInputRequest request)
    {
        string prompt =
            string.IsNullOrWhiteSpace(request.AgentResponse.Text) || request.AgentResponse.ResponseId is not null ?
                "INPUT:" :
                request.AgentResponse.Text;

        string? userInput;
        do
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"{prompt} ");
            Console.ForegroundColor = ConsoleColor.White;
            userInput = Console.ReadLine();
        }
        while (string.IsNullOrWhiteSpace(userInput));

        return new ChatMessage(ChatRole.User, userInput);
    }

    private static async ValueTask DownloadFileContentAsync(string filename, BinaryData content)
    {
        string filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));
        filePath = Path.ChangeExtension(filePath, ".png");

        await File.WriteAllBytesAsync(filePath, content.ToArray()).ConfigureAwait(false);

        Process.Start(
            new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C start {filePath}"
            });
    }
}
