using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Responses;

using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Declarative;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;


internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        Uri foundryEndpoint = new(config["AZURE_AI_PROJECT_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set. Run: dotnet user-secrets set \"AZURE_AI_PROJECT_ENDPOINT\" \"<value>\""));
        string yamlPath = config["WORKFLOW_YAML_PATH"] ?? "workflow.yaml";

        WorkflowFactory workflowFactory = new(yamlPath, foundryEndpoint);
        WorkflowRunner runner = new();
        await runner.ExecuteAsync(workflowFactory.CreateWorkflow, "junior developer");    
    }
}
    

