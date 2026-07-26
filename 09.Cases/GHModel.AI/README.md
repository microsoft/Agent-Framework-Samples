# GHModel.AI — Multi-Agent Workflow Demos with Microsoft Agent Framework

[![Python](https://img.shields.io/badge/Python-3.10+-blue.svg)](https://python.org)
[![.NET](https://img.shields.io/badge/.NET-8.0+-purple.svg)](https://dotnet.microsoft.com)
[![Agent Framework](https://img.shields.io/badge/Microsoft-Agent%20Framework-0078D4.svg)](https://github.com/microsoft/agent-framework)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> A complete collection of examples for building, testing, and monitoring multi-agent workflows with Microsoft Agent Framework, covering **AG-UI**, **DevUI**, and **OpenTelemetry** capabilities.

---

## 📖 Overview

This project demonstrates how to efficiently develop multi-agent applications using [Microsoft Agent Framework](https://github.com/microsoft/agent-framework). Using a "Travel Assistant" scenario, the project showcases:

1. **Application Integration**: Quickly integrate workflows into Web/mobile apps via the AG-UI protocol.
2. **Local Debugging**: Visually test workflow execution paths using DevUI.
3. **Performance Monitoring**: Collect end-to-end Traces, Metrics, and Logs with OpenTelemetry.

Examples are provided in both **Python** and **.NET** implementations for developers across different tech stacks.

---

## 🏗️ Project Structure

```
GHModel.AI/
├── GHModel.Python.AI/                         # Python Examples
│   ├── GHModel.Python.AI.Workflow.AGUI/       # AG-UI Client/Server
│   │   ├── GHModel.Python.AI.Workflow.AGUI.Server/
│   │   └── GHModel.Python.AI.Workflow.AGUI.Client/
│   ├── GHModel.Python.AI.Workflow.DevUI/      # DevUI Local Debugging
│   └── GHModel.Python.AI.Workflow.OpenTelemetry/ # OpenTelemetry Integration
│
├── GHModel.dotNET.AI/                         # .NET Examples
│   ├── GHModel.dotNET.AI.Workflow.AGUI/       # AG-UI Server + CopilotKit
│   ├── GHModel.dotNET.AI.Workflow.DevUI/      # DevUI ASP.NET Core Integration
│   └── GHModel.dotNET.AI.Workflow.OpenTelemetry/ # OpenTelemetry Console App
│
└── README.md
```

---

## 🔧 Architecture

### Workflow Design

The project uses a dual-agent sequential workflow:

```
┌──────────────────┐        ┌──────────────────┐
│  FrontDesk Agent │  ───▶  │ Concierge Agent  │
│  (Recommendations)│        │  (Review)        │
└──────────────────┘        └──────────────────┘
```

- **FrontDesk Agent**: Provides travel activity/location recommendations based on user requests.
- **Concierge Agent**: Reviews recommendations to ensure authentic, local, non-touristy experiences.

### Core Components

| Component | Purpose | Documentation |
|-----------|---------|---------------|
| **AG-UI** | Standardized AI Agent interface protocol with SSE streaming, state sync, human-in-the-loop | [AG-UI Integration](https://learn.microsoft.com/en-us/agent-framework/integrations/ag-ui/) |
| **DevUI** | Lightweight web debugging interface with entity auto-discovery and OpenAI-compatible API | [DevUI Samples](https://github.com/microsoft/agent-framework/tree/main/python/samples/getting_started/devui) |
| **OpenTelemetry** | Observability based on OpenTelemetry semantic conventions, supporting Trace/Metrics/Logs | [Observability Guide](https://github.com/microsoft/agent-framework/tree/main/python/samples/getting_started/observability) |

---

## 🚀 Getting Started

### Prerequisites

- Python 3.10+ / .NET 10.0+
- GitHub Models API access (or Azure OpenAI / OpenAI)
- Docker (optional, for running Aspire Dashboard)

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/GHModel.AI.git
cd GHModel.AI
```

### 2. Configure Environment Variables

Set these environment variables in your shell or in the `.env` location used by
the example you are running. The .NET DevUI example loads
`09.Cases/GHModel.AI/.env`:

```env
GITHUB_ENDPOINT=https://models.github.ai/inference
# Fine-grained PAT with "Models: read" permission
GITHUB_TOKEN=your_github_token
GITHUB_MODEL_ID=openai/gpt-4.1-mini
```

---

## 📦 Examples

### 1️⃣ AG-UI — Build Web Agent Interfaces

AG-UI provides a standardized HTTP + SSE protocol for easy frontend integration.

#### Python Server

```bash
cd GHModel.Python.AI/GHModel.Python.AI.Workflow.AGUI/GHModel.Python.AI.Workflow.AGUI.Server
pip install agent-framework agent-framework-ag-ui fastapi uvicorn python-dotenv
python main.py
# Server starts at http://127.0.0.1:8888
```

#### Python CLI Client

```bash
cd GHModel.Python.AI/GHModel.Python.AI.Workflow.AGUI/GHModel.Python.AI.Workflow.AGUI.Client
python main.py
# Interactive chat client, type :q to exit
```

#### Python CopilotKit Client

```bash
cd GHModel.Python.AI/GHModel.Python.AI.Workflow.AGUI/GHModel.Python.AI.Workflow.AGUI.CopilotKit
npm install
npm run dev
```

#### .NET Server

```bash
cd GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.AGUI/GHModel.dotNET.AI.Workflow.AGUI.Server
dotnet run
# Server starts at configured port
```

#### .NET Client

```bash
cd GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.AGUI/GHModel.dotNET.AI.Workflow.AGUI.Client
dotnet run
```

#### .NET CopilotKit Client

```bash
cd GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.AGUI/GHModel.dotNET.AI.Workflow.AGUI.CopilotKit
npm install
npm run dev
# Open http://localhost:3000 in browser
```

#### Code Snippets

**Python Server**

```python
# Server — Register AG-UI endpoint
from agent_framework_ag_ui import add_agent_framework_fastapi_endpoint
from workflow import workflow

app = FastAPI()
agent = workflow.as_agent(name="Travel Agent")
add_agent_framework_fastapi_endpoint(app, agent, "/")
```

**.NET Server**

```csharp
// Program.cs — ASP.NET Core AG-UI endpoint registration
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAGUI();

var app = builder.Build();

// Create workflow agent
AIAgent workflowAgent = ChatClientAgentFactory.CreateTravelAgenticChat();

// Map AG-UI endpoint
app.MapAGUI("/", workflowAgent);
await app.RunAsync();
```

```csharp
// ChatClientAgentFactory.cs — Build dual-agent workflow
var chatClient = openAIClient.GetChatClient(modelId).AsIChatClient();

AIAgent frontDeskAgent = chatClient.CreateAIAgent(
    name: "FrontDesk",
    instructions: "You are a Front Desk Travel Agent...");

AIAgent reviewerAgent = chatClient.CreateAIAgent(
    name: "Concierge",
    instructions: "You are a hotel concierge...");

var workflow = new WorkflowBuilder(frontDeskAgent)
    .AddEdge(frontDeskAgent, reviewerAgent)
    .Build();

return workflow.AsAgent("travel-workflow", "travel recommendation workflow");
```

#### AG-UI Supported Features

- ✅ Streaming responses (SSE)
- ✅ Backend tool rendering
- ✅ Human-in-the-Loop approvals
- ✅ Shared state synchronization
- ✅ Seamless [CopilotKit](https://copilotkit.ai/) integration

---

### 2️⃣ DevUI — Visual Workflow Debugging

DevUI provides an out-of-the-box web interface for testing agents and workflows without writing frontend code.

#### Python

```bash
cd GHModel.Python.AI/GHModel.Python.AI.Workflow.DevUI
pip install agent-framework agent-framework-devui python-dotenv
python main.py
# Browser opens automatically at http://localhost:8090
```

#### .NET

```bash
cd GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.DevUI
dotnet run
# DevUI: http://localhost:50518/devui
# OpenAI API: http://localhost:50518/v1/responses
```

Select `gh-model-workflow` in DevUI to run the sequential FrontDesk and
Concierge workflow.

#### Code Snippets

```python
# Python — Start DevUI in one line
from agent_framework.devui import serve
serve(entities=[workflow], port=8090, auto_open=True, tracing_enabled=True)
```

```csharp
// .NET — ASP.NET Core integration
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDevUI();
}
builder.AddAIAgent("FrontDesk", FrontDeskAgentInstructions);
builder.AddWorkflow("gh-model-workflow", ...);

builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

app.MapOpenAIResponses();
app.MapOpenAIConversations();

if (app.Environment.IsDevelopment())
{
    app.MapDevUI();
}
```

---

### 3️⃣ OpenTelemetry — End-to-End Observability

Agent Framework natively supports OpenTelemetry for exporting Traces, Metrics, and Logs to Application Insights, Aspire Dashboard, or any APM platform.

#### Python

```bash
cd GHModel.Python.AI/GHModel.Python.AI.Workflow.OpenTelemetry
pip install agent-framework opentelemetry-exporter-otlp-proto-grpc python-dotenv
python main.py
# Interactive chat, type exit to quit
```

#### .NET

```bash
cd GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.OpenTelemetry

# Start Aspire Dashboard (optional)
docker run --rm -d -p 18888:18888 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:latest

dotnet run
# View traces at http://localhost:18888
```

#### Environment Variables

```env
ENABLE_OTEL=true
ENABLE_SENSITIVE_DATA=true               # Enable sensitive data logging in dev
OTLP_ENDPOINT=http://localhost:4317       # Aspire Dashboard / OTLP Collector
APPLICATIONINSIGHTS_CONNECTION_STRING=... # Azure Application Insights (optional)
```

#### Code Snippets

```python
# Python — Enable telemetry in one line
from agent_framework.observability import setup_observability
from agent_framework import setup_logging

setup_observability()
setup_logging()
```

```csharp
// .NET — OpenTelemetry configuration
var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("*Microsoft.Agents.AI")
    .AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"))
    .Build();
```

---

## 📊 Monitoring & Visualization

### Aspire Dashboard

View OpenTelemetry data locally without an Azure account:

```bash
docker run --rm -d -p 18888:18888 -p 4317:18889 mcr.microsoft.com/dotnet/aspire-dashboard:latest
```

Access http://localhost:18888 to view Traces, Logs, and Metrics.

### Application Insights

For production environments, use Azure Application Insights:

1. Create an Application Insights resource
2. Set the `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable
3. Use Transaction Search in Azure Portal to query by Operation ID

### Grafana Dashboards

Azure Managed Grafana provides pre-built dashboards:

- [Agent Overview](https://aka.ms/amg/dash/af-agent)
- [Workflow Overview](https://aka.ms/amg/dash/af-workflow)

---

## 🛠️ Development Guide

### Create Custom Agents

```python
from agent_framework.openai import OpenAIChatClient

chat_client = OpenAIChatClient(
    base_url=os.environ.get("GITHUB_ENDPOINT"),
    api_key=os.environ.get("GITHUB_TOKEN"),
    model_id=os.environ.get("GITHUB_MODEL_ID")
)

my_agent = chat_client.create_agent(
    name="MyAgent",
    instructions="Your custom instructions here..."
)
```

### Build Workflows

```python
from agent_framework import WorkflowBuilder, AgentExecutor

workflow = (
    WorkflowBuilder()
    .set_start_executor(AgentExecutor(agent_a, id="agent_a"))
    .add_edge(agent_a_executor, agent_b_executor)
    .build()
)
```

---

## 📚 References

| Resource | Link |
|----------|------|
| Agent Framework GitHub | https://github.com/microsoft/agent-framework |
| AG-UI Protocol Spec | https://docs.ag-ui.com/introduction |
| CopilotKit Integration | https://docs.copilotkit.ai/microsoft-agent-framework |
| OpenTelemetry Python | https://opentelemetry.io/docs/languages/python/ |
| Aspire Dashboard | https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard/standalone |
| Application Insights | https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview |

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🤝 Contributing

Contributions are welcome! Feel free to submit Issues and Pull Requests. For questions or suggestions, please open an issue on GitHub.
