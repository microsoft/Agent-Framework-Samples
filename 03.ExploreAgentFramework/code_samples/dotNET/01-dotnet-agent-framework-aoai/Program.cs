using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using DotNetEnv;

Env.Load("../../../../.env");

var aoai_endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var aoai_model_id = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

Console.WriteLine($"Using Azure OpenAI Endpoint: {aoai_endpoint}");
Console.WriteLine($"Using Azure OpenAI Model Deployment: {aoai_model_id}");

AIAgent agent = new AzureOpenAIClient(
    new Uri(aoai_endpoint),
    new AzureCliCredential())
     .GetChatClient(aoai_model_id)
     .AsAIAgent(instructions: "You are a helpful assistant.",name:"MAFDemoAgent");


Console.WriteLine(await agent.RunAsync("Write a haiku about Agent Framework."));

await foreach (var update in agent.RunStreamingAsync("Write a haiku about Agent Framework."))
{
    Console.Write(update);
}



