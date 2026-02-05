#pragma warning disable MEAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates

using System;
using System.Linq;

using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using DotNetEnv;

Env.Load("../../../../../.env");

var azure_foundry_endpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT") ?? throw new InvalidOperationException("AZURE_FOUNDRY_PROJECT_ENDPOINT is not set.");
var azure_foundry_model_id = Environment.GetEnvironmentVariable("AZURE_AI_MODEL_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";


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
// var threadWithRequiredApproval = await agentWithRequiredApproval.GetNewThreadAsync();
var sessionWithRequiredApproval = await agentWithRequiredApproval.CreateSessionAsync();
var response = await agentWithRequiredApproval.RunAsync("Please summarize the Azure AI Agent documentation related to MCP Tool calling?", sessionWithRequiredApproval);
var userInputRequests = response.UserInputRequests.ToList();

while (userInputRequests.Count > 0)
{
    // Ask the user to approve each MCP call request.
    // For simplicity, we are assuming here that only MCP approval requests are being made.
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

    userInputRequests = response.UserInputRequests.ToList();
}

Console.WriteLine($"\nAgent: {response}");
