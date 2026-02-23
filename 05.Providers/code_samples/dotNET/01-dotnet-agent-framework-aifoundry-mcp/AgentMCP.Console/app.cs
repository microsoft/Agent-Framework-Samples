// File-based run: dotnet run app.cs
#:property UserSecretsId d0e13627-3df1-4b64-aec6-0fb17f50a14d
#:property EnablePreviewFeatures true
#:package Azure.Identity@1.17.1
#:package Azure.AI.Agents.Persistent@1.2.0-beta.8
#:package ModelContextProtocol@0.6.0-preview.1
#:package System.Linq.Async@7.0.0
#:package Azure.AI.Projects@1.2.0-beta.5
#:package Azure.AI.Projects.OpenAI@1.0.0-beta.5
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Agents.AI@1.0.0-rc1
#:package Microsoft.Agents.AI.AzureAI.Persistent@1.0.0-preview.260219.1
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0

#pragma warning disable MEAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates

using System;
using System.Linq;

using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var azure_foundry_endpoint = config["AZURE_AI_PROJECT_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_FOUNDRY_PROJECT_ENDPOINT is not set.");
var azure_foundry_model_id = config["AZURE_AI_MODEL_DEPLOYMENT_NAME"] ?? "gpt-4.1-mini";


var persistentAgentsClient = new PersistentAgentsClient(azure_foundry_endpoint, new AzureCliCredential());
var mcpToolWithApproval = new HostedMcpServerTool(
    serverName: "microsoft_learn",
    serverAddress: "https://learn.microsoft.com/api/mcp")
{
    AllowedTools = ["microsoft_docs_search"],
    ApprovalMode = HostedMcpServerToolApprovalMode.AlwaysRequire
};

// Create an agent based on Azure OpenAI Responses as the backend.
AIAgent agentWithRequiredApproval = await persistentAgentsClient.CreateAIAgentAsync(
    model: azure_foundry_model_id,
    options: new()
    {
        Name = "MicrosoftLearnAgentWithApproval",
        ChatOptions = new()
        {
            Instructions = "You answer questions by searching the Microsoft Learn content only.",
            Tools = [mcpToolWithApproval]
        },
    });

// You can then invoke the agent like any other AIAgent.
var sessionWithRequiredApproval = await agentWithRequiredApproval.CreateSessionAsync();
var response = await agentWithRequiredApproval.RunAsync("Please summarize the Azure AI Agent documentation related to MCP Tool calling?", sessionWithRequiredApproval);
var userInputRequests = response.Messages.SelectMany(m => m.Contents).OfType<McpServerToolApprovalRequestContent>().ToList();

while (userInputRequests.Count > 0)
{
    // Ask the user to approve each MCP call request.
    var userInputResponses = userInputRequests
        .OfType<McpServerToolApprovalRequestContent>()
        .Select(approvalRequest =>
        {
            Console.WriteLine($"""
                The agent would like to invoke the following MCP Tool, please reply Y to approve.
                ServerName: {approvalRequest.ToolCall.ServerName}
                Name: {approvalRequest.ToolCall.ToolName}
                Arguments: {string.Join(", ", approvalRequest.ToolCall.Arguments?.Select(x => $"{x.Key}: {x.Value}") ?? [])}
                """);
            return new ChatMessage(ChatRole.User, [approvalRequest.CreateResponse(Console.ReadLine()?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false)]);
        })
        .ToList();

    // Pass the user input responses back to the agent for further processing.
    response = await agentWithRequiredApproval.RunAsync(userInputResponses, sessionWithRequiredApproval);

    userInputRequests = response.Messages.SelectMany(m => m.Contents).OfType<McpServerToolApprovalRequestContent>().ToList();
}

Console.WriteLine($"\nAgent: {response}");
