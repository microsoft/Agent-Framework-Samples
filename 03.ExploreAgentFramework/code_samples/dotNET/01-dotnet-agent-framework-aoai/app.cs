// File-based run: dotnet run app.cs
#:property UserSecretsId 2e421ed5-5bee-47c3-9608-7d42f9dfed00
#:package Azure.AI.OpenAI@2.1.0
#:package Azure.Identity@1.17.1
#:package Microsoft.Extensions.AI.OpenAI@10.3.0
#:package Microsoft.Agents.AI.OpenAI@1.0.0-rc1
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.0
#:package Microsoft.Extensions.Configuration.EnvironmentVariables@10.0.0

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var aoai_endpoint = config["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var aoai_model_id = config["AZURE_OPENAI_RESPONSES_DEPLOYMENT_NAME"] ?? "gpt-4.1-mini";

Console.WriteLine($"Using Azure OpenAI Endpoint: {aoai_endpoint}");
Console.WriteLine($"Using Azure OpenAI Model Deployment: {aoai_model_id}");

AIAgent agent = new AzureOpenAIClient(
    new Uri(aoai_endpoint),
    new AzureCliCredential())
     .GetChatClient(aoai_model_id)
     .AsAIAgent(instructions: "You are a helpful assistant.", name: "MAFDemoAgent");


Console.WriteLine(await agent.RunAsync("Write a haiku about Agent Framework."));

await foreach (var update in agent.RunStreamingAsync("Write a haiku about Agent Framework."))
{
    Console.Write(update);
}
