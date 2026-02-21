
using System;
using System.ComponentModel;
using System.ClientModel;
using AGUIDojoServer;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;  
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using Microsoft.AspNetCore.HttpLogging;
using OpenAI;
using OpenAI.Chat;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.RequestBody
        | HttpLoggingFields.ResponsePropertiesAndHeaders | HttpLoggingFields.ResponseBody;
    logging.RequestBodyLogLimit = int.MaxValue;
    logging.ResponseBodyLogLimit = int.MaxValue;
});

builder.Services.AddHttpClient().AddLogging();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Add(AGUIDojoServerSerializerContext.Default));
builder.Services.AddAGUI();

ChatClientAgentFactory.Initialize(builder.Configuration);

WebApplication app = builder.Build();


// Map the AG-UI agent endpoint
app.MapAGUI("/", ChatClientAgentFactory.CreateTravelAgenticChat());
await app.RunAsync();


public partial class Program;