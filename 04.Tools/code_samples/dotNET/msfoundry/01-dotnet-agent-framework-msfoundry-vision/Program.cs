using System;
using System.Linq;
using System.IO;
using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
 using DotNetEnv;

 Env.Load("../../../../../.env");


var azure_foundry_endpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT") ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var azure_foundry_model_id = "gpt-4o";

var imgPath ="../../../files/home.png";

const string AgentName = "Vision-Agent";
const string AgentInstructions = "You are my furniture sales consultant, you can find different furniture elements from the pictures and give me a purchase suggestion";


async Task<byte[]> OpenImageBytesAsync(string path)
{
	return await File.ReadAllBytesAsync(path);
}

var imageBytes = await OpenImageBytesAsync(imgPath);

AIProjectClient aiProjectClient = new(
    new Uri(azure_foundry_endpoint),
    new AzureCliCredential());


AIAgent agent = await aiProjectClient.CreateAIAgentAsync(
    name: AgentName,
    creationOptions: new AgentVersionCreationOptions(
        new PromptAgentDefinition(model: azure_foundry_model_id)
        {
            Instructions = AgentInstructions
        })
);


ChatMessage userMessage = new ChatMessage(ChatRole.User, [
	new TextContent("Can you identify the furniture items in this image and suggest which ones would fit well in a modern living room?"), new DataContent(imageBytes, "image/png")
]);



AgentSession session = await agent.CreateSessionAsync();


Console.WriteLine(await agent.RunAsync(userMessage, session));

