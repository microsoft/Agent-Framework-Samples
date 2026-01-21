# Microsoft Agent Framework Samples

A comprehensive hands-on guide to building intelligent agents using the Microsoft Agent Framework. This repository contains practical examples, tutorials, and code samples that demonstrate how to create powerful AI agents using both Python and .NET implementations.

## 🚀 What You'll Learn

This repository provides step-by-step tutorials and real-world examples covering:

- **Agent Foundations**: Core concepts and architecture of the Microsoft Agent Framework
- **Creating Your First Agent**: Build a simple travel planning agent from scratch
- **Framework Exploration**: Deep dive into different providers and configurations
- **Tools Integration**: Implement vision, code interpretation, and custom tools
- **Provider Patterns**: Work with MCP (Model Context Protocol) and Agent-to-Agent communication
- **RAG Implementation**: Build knowledge-enhanced agents with file search capabilities
- **Multi-Agent Systems**: Orchestrate multiple agents working together
- **Workflow Management**: Create complex agent workflows and pipelines

## 📁 Repository Structure

| Directory | Description | .NET Code Samples | Python Code Samples |
|-----------|-------------|-------------------|---------------------|
| **[00.ForBeginners](./00.ForBeginners/README.md)** | **Beginner-friendly Microsoft Agent Framework examples extending [AI Agents for Beginners](https://github.com/microsoft/ai-agents-for-beginners)** | [Travel Agent](./00.ForBeginners/01-intro-to-ai-agents/code_samples/dotnet-agent-framework-travelagent/)<br/>[Basic Agent](./00.ForBeginners/02-explore-agentic-frameworks/code_samples/dotnet-agent-framework-basicagent/)<br/>[Design Patterns](./00.ForBeginners/03-agentic-design-patterns/code_samples/dotnet-agent-framework-basicagent/)<br/>[Tool Use](./00.ForBeginners/04-tool-use/code_samples/dotnet-agent-framework-ghmodels-tool/)<br/>[RAG Search](./00.ForBeginners/05-agentic-rag/code_samples/dotnet-agent-framework-msfoundry-file-search/)<br/>[Planning](./00.ForBeginners/07-planning-design/code_samples/dotnet-agent-framrwork-ghmodel-planningdesign/)<br/>[Multi-Agent](./00.ForBeginners/08-multi-agent/code_samples/dotnet-agent-framework-ghmodel-workflow-multi-agents/) | [Travel Agent](./00.ForBeginners/01-intro-to-ai-agents/code_samples/python-agent-framework-travelagent.ipynb)<br/>[Basic Agent](./00.ForBeginners/02-explore-agentic-frameworks/code_samples/python-agent-framework-basicagent.ipynb)<br/>[Design Patterns](./00.ForBeginners/03-agentic-design-patterns/code_samples/python-agent-framework-ghmodel-basicagent.ipynb)<br/>[Tool Use](./00.ForBeginners/04-tool-use/code_samples/python-agent-framework-ghmodel-tools.ipynb)<br/>[RAG Search](./00.ForBeginners/05-agentic-rag/code_samples/python-agent-framework-msfoundry-file-search.ipynb)<br/>[Planning](./00.ForBeginners/07-planning-design/code_samples/python-agent-framrwork-ghmodel-planningdesign.ipynb)<br/>[Multi-Agent](./00.ForBeginners/08-multi-agent/code_samples/python-agent-framework-ghmodel-workflow-multi-agents.ipynb) |
| **[01.AgentFoundation](./01.AgentFoundation/README.md)** | Core concepts and architecture of Microsoft Agent Framework | *Documentation Only* | *Documentation Only* |
| **[02.CreateYourFirstAgent](./02.CreateYourFirstAgent/README.md)** | Build your first travel planning agent from scratch | [Travel Agent with GitHub Models](./02.CreateYourFirstAgent/code_samples/dotNET/dotnet-travelagent-ghmodel.ipynb) | [Travel Agent with GitHub Models](./02.CreateYourFirstAgent/code_samples/python/python-travelagent-ghmodel.ipynb) |
| **[03.ExploreAgentFramework](./03.ExploerAgentFramework/README.md)** | Deep dive into different providers and configurations | [Azure OpenAI](./03.ExploerAgentFramework/code_samples/dotNET/01-dotnet-agent-framework-aoai/)<br/>[GitHub Models](./03.ExploerAgentFramework/code_samples/dotNET/02-dotnet-agent-framework-ghmodel/)<br/>[MS Foundry](./03.ExploerAgentFramework/code_samples/dotNET/03-dotnet-agent-framework-msfoundry/)<br/>[Foundry Local](./03.ExploerAgentFramework/code_samples/dotNET/04-dotnet-agent-framework-foundrylocal/) | [Azure OpenAI](./03.ExploerAgentFramework/code_samples/python/01-python-agent-framework-aoai.ipynb)<br/>[GitHub Models](./03.ExploerAgentFramework/code_samples/python/02-python-agent-framrwork-ghmodel.ipynb)<br/>[MS Foundry](./03.ExploerAgentFramework/code_samples/python/03-python-agent-framework-msfoundry.ipynb)<br/>[Foundry Local](./03.ExploerAgentFramework/code_samples/python/04-python-agent-framrwork-foundrylocal.ipynb) |
| **[04.Tools](./04.Tools/README.md)** | Vision, code interpretation, and custom tool integration | [Vision](./04.Tools/code_samples/dotNET/foundry/01-dotnet-agent-framework-aifoundry-vision.ipynb)<br/>[Code Interpreter](./04.Tools/code_samples/dotNET/foundry/02-dotnet-agent-framework-aifoundry-code-interpreter.ipynb)<br/>[Bing Grounding](./04.Tools/code_samples/dotNET/foundry/03-dotnet-agent-framework-aifoundry-binggrounding.ipynb)<br/>[File Search](./04.Tools/code_samples/dotNET/foundry/04-dotnet-agent-framework-aifoundry-file-search.ipynb) | [Vision](./04.Tools/code_samples/python/foundry/01.python-agent-framework-aifoundry-vision.ipynb)<br/>[Code Interpreter](./04.Tools/code_samples/python/foundry/02.python-agent-framework-aifoundry-code-interpreter.ipynb)<br/>[Bing Grounding](./04.Tools/code_samples/python/foundry/03.python-agent-framework-aifoundry-binggrounding.ipynb)<br/>[File Search](./04.Tools/code_samples/python/foundry/04.python-agent-framework-aifoundry-file-search.ipynb) |
| **[05.Providers](./05.Providers/README.md)** | MCP (Model Context Protocol) and Agent-to-Agent communication | [MCP with Microsoft Learn](./05.Providers/code_samples/dotNET/01-dotnet-agent-framework-aifoundry-mcp/AgentMCP.Console/)| [MCP with Microsoft Learn](./05.Providers/code_samples/python/01-python-agent-framework-aifoundry-mcp.ipynb) |
| **[06.RAGs](./06.RAGs/README.md)** | Knowledge-enhanced agents with file search capabilities | [File Search RAG](./06.RAGs/code_samples/dotNET/dotnet-agent-framework-aifoundry-file-search.ipynb) | [File Search RAG](./06.RAGs/code_samples/python/python-agent-framework-aifoundry-file-search.ipynb) |
| **[07.Workflow](./07.Workflow/README.md)** | Complex agent workflows and orchestration patterns | [Basic Workflow](./07.Workflow/code_samples/dotNET/01.dotnet-agent-framework-workflow-ghmodel-basic.ipynb)<br/>[Sequential](./07.Workflow/code_samples/dotNET/02.dotnet-agent-framework-workflow-ghmodel-sequential.ipynb)<br/>[Concurrent](./07.Workflow/code_samples/dotNET/03.dotnet-agent-framework-workflow-ghmodel-concurrent.ipynb)<br/>[Conditional](./07.Workflow/code_samples/dotNET/04.dotnet-agent-framework-workflow-aifoundry-condition.ipynb) | [Basic Workflow](./07.Workflow/code_samples/python/01.python-agent-framework-workflow-ghmodel-basic.ipynb)<br/>[Sequential](./07.Workflow/code_samples/python/02.python-agent-framework-workflow-ghmodel-sequential.ipynb)<br/>[Concurrent](./07.Workflow/code_samples/python/03.python-agent-framework-workflow-ghmodel-concurrent.ipynb)<br/>[Conditional](./07.Workflow/code_samples/python/04.python-agent-framework-workflow-aifoundry-condition.ipynb) |
| **[08.EvaluationAndTracing](./08.EvaluationAndTracing/README.md)** | Agent evaluation, debugging, and observability tools | *Python Only* | [Basic Agent DevUI](./08.EvaluationAndTracing/python/basic_agent_workflow_devui/)<br/>[Multi-Agent DevUI](./08.EvaluationAndTracing/python/multi_workflow_ghmodel_devui/)<br/>[Foundry Agent DevUI](./08.EvaluationAndTracing/python/multi_workflow_aifoundry_devui/)<br/>[Foundry Local DevUI](./08.EvaluationAndTracing/python/multi_workflow_foundrylocal_devui/)<br/>[Aspire Tracing](./08.EvaluationAndTracing/python/tracer_aspire/) |
| **[09.Cases](./09.Cases/README.md)** | Real-world case studies combining Foundry workflows with production-ready multi-agent applications | [Microsoft Foundry with AITK & MAF](./09.Cases/MicrosoftFoundryWithAITKAndMAF/README.md)<br/>[GHModel Multi-Agent (.NET)](./09.Cases/GHModel.AI/GHModel.dotNET.AI/) | [Microsoft Foundry with AITK & MAF](./09.Cases/MicrosoftFoundryWithAITKAndMAF/README.md)<br/>[GHModel Multi-Agent (Python)](./09.Cases/GHModel.AI/GHModel.Python.AI/)<br/>[Agentic Marketing Content Generation](./09.Cases/AgenticMarketingContentGen/README.md) |

## 🛠 Prerequisites

***Note: This is installation guideline***

> ⚠️ **Important Notice**: Microsoft Agent Framework is currently in the **development/preview stage**. Since the framework APIs and features may change frequently, **we strongly recommend building from source** rather than using NuGet packages to ensure you have the latest updates and bug fixes.

### Python Environment
- Python 3.10 or higher
- Install dependencies: 

```bash
pip install -r ./Installation/requirement.txt -U
```

**Build from Source (Recommended):**

```bash
git clone https://github.com/microsoft/agent-framework.git
cd agent-framework/python
pip install -e .
```

### .NET Environment
- .NET 9.0 or higher
- Visual Studio 2022 or VS Code with C# extension

**Build from Source (Recommended):**

```bash
git clone https://github.com/microsoft/agent-framework.git
cd agent-framework/dotnet && dotnet build agent-framework-dotnet.slnx
```

After building, reference the local project in your notebooks or applications instead of NuGet packages. This ensures compatibility with the latest framework changes.


## 💻 Platform-Specific Setup

### Windows ARM64 Configuration

If you're running on Windows ARM64, you may need to configure OpenSSL for certain dependencies:

```bash
git config --global core.longpaths true
winget install ShiningLight.OpenSSL.Dev
$env:OPENSSL_DIR="C:\Program Files\OpenSSL-Win64-ARM"
$env:OPENSSL_LIB_DIR="C:\Program Files\OpenSSL-Win64-ARM\lib\VC\arm64\MT"
$env:OPENSSL_STATIC="1"
```

### Cross-Platform Compatibility
- **Linux**: Standard pip installation
- **macOS**: Homebrew for system dependencies
- **Windows x64**: Standard Windows installation

### Required Services
- Azure OpenAI Service and Microsoft Foundry
- GitHub Models (for some examples)
- Azure CLI (authenticated)
- Azure Developer CLI (authenticated)

## 🚀 Quick Start

### Environment Setup

Create a `.env` file in the root directory with your configurations:

```env
GITHUB_TOKEN="Your GitHub Models Token"
GITHUB_ENDPOINT="Your GitHub Models Endpoint"
GITHUB_MODEL_ID="Your GitHub Model ID"

AZURE_OPENAI_ENDPOINT="Your Azure OpenAI Endpoint"
AZURE_OPENAI_CHAT_DEPLOYMENT_NAME ="Your Azure OpenAI Model Deployment Name"


FOUNDRYLOCAL_ENDPOINT="Your Foundry Local Endpoint http://localhost:5272/v1"
FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME="Your Foundry Local Model Deployment Name"


AZURE_AI_PROJECT_ENDPOINT ="Your Azure AI Foundry Project Endpoint"
AZURE_AI_MODEL_DEPLOYMENT_NAME ="Your Azure AI Foundry Project Deployment Name"

BING_CONNECTION_ID="Your Bing Connection ID"
BING_CONNECTION_NAME="Your Bing Connection Name"

OTEL_EXPORTER_OTLP_ENDPOINT="Your OpenTelemetry Collector Endpoint e.g. http://localhost:4317"
```


## 📚 Tutorial Progression

### Getting Started Level
0. **00.ForBeginners** - Comprehensive beginner tutorials with Microsoft Agent Framework examples

### Foundation Level
1. **01.AgentFoundation** - Understand the core concepts and architecture
2. **02.CreateYourFirstAgent** - Build your first travel planning agent

### Intermediate Level
3. **03.ExploreAgentFramework** - Explore different providers (Azure OpenAI, GitHub Models, AI Foundry)
4. **04.Tools** - Add vision, code interpretation, and custom tool capabilities
5. **06.RAGs** - Implement knowledge-enhanced agents with file search

### Advanced Level
6. **05.Providers** - Master MCP (Model Context Protocol) and Agent-to-Agent communication
7. **07.Workflow** - Create complex agent workflows and orchestration patterns
8. **08.EvaluationAndTracing** - Learn evaluation, debugging, and observability tools for agents

## 🔧 Key Features Demonstrated

- **Multiple Provider Support**: Azure OpenAI, GitHub Models, Microsoft Foundry
- **Tool Integration**: Vision analysis, code interpretation, custom functions
- **RAG Capabilities**: File search and knowledge base integration
- **Multi-Agent Orchestration**: Sequential and collaborative agent patterns
- **MCP Integration**: Model Context Protocol for enhanced capabilities
- **Streaming Responses**: Real-time agent interactions
- **Persistent Agents**: Stateful agent conversations
- **Evaluation & Debugging**: DevUI for visual debugging and observability tools for tracing

## 🤝 Contributing

We welcome contributions! Please feel free to submit issues, feature requests, or pull requests.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🔗 Resources

- [Microsoft Agent Framework Documentation](https://github.com/microsoft/agent-framework)
- [Azure AI Services](https://azure.microsoft.com/en-us/products/ai-services)
- [Microsoft Foundry](https://azure.microsoft.com/en-us/products/ai-foundry)
- [GitHub Models](https://github.com/marketplace/models)

## 🆘 Support

If you encounter any issues or have questions:
1. Check the individual README files in each chapter directory
2. Review the code samples for implementation details
3. Open an issue in this repository
4. Consult the official Microsoft Agent Framework documentation

---

**Start your journey with Microsoft Agent Framework today!** 🚀