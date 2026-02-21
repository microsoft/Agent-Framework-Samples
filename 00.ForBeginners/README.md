# Microsoft Agent Framework Examples for AI Agents Beginners

This section provides comprehensive Microsoft Agent Framework examples that extend and complement the content from [Microsoft's AI Agents for Beginners](https://github.com/microsoft/ai-agents-for-beginners) curriculum. These practical code samples demonstrate how to build intelligent agents using both Python and .NET implementations of the Microsoft Agent Framework.

## 🎯 Overview

The examples in this directory are designed to provide hands-on experience with Microsoft Agent Framework, covering fundamental concepts through advanced multi-agent systems. Each lesson includes both Python and .NET code samples to accommodate different development preferences and environments.

## 📚 Learning Path

### 01. Introduction to AI Agents
Learn the foundational concepts of AI agents and get started with your first Microsoft Agent Framework implementation.

**Code Samples:**
- **Python:** [`python-agent-framework-travelagent.ipynb`](./01-intro-to-ai-agents/code_samples/python-agent-framework-travelagent.ipynb)
- **.NET:** [`dotnet-agent-framework-travelagent.ipynb`](./01-intro-to-ai-agents/code_samples/dotnet-agent-framework-travelagent.ipynb)

### 02. Explore Agentic Frameworks
Dive deeper into the Microsoft Agent Framework architecture and understand different implementation patterns.

**Code Samples:**
- **Python:** [`python-agent-framework-basicagent.ipynb`](./02-explore-agentic-frameworks/code_samples/python-agent-framework-basicagent.ipynb)
- **.NET:** [`dotnet-agent-framework-basicagent.ipynb`](./02-explore-agentic-frameworks/code_samples/dotnet-agent-framework-basicagent.ipynb)

### 03. Agentic Design Patterns
Explore common design patterns and best practices for building robust AI agents with GitHub Models integration.

**Code Samples:**
- **Python:** [`python-agent-framework-ghmodel-basicagent.ipynb`](./03-agentic-design-patterns/code_samples/python-agent-framework-ghmodel-basicagent.ipynb)
- **.NET:** [`dotnet-agent-framework-ghmodel-basicagent.ipynb`](./03-agentic-design-patterns/code_samples/dotnet-agent-framework-ghmodel-basicagent.ipynb)

### 04. Tool Use and Integration
Learn how to enhance your agents with external tools and capabilities using GitHub Models.

**Code Samples:**
- **Python:** [`python-agent-framework-ghmodel-tools.ipynb`](./04-tool-use/code_samples/python-agent-framework-ghmodel-tools.ipynb)
- **.NET:** [`dotnet-agent-framework-ghmodels-tool.ipynb`](./04-tool-use/code_samples/dotnet-agent-framework-ghmodels-tool.ipynb)

### 05. Agentic RAG (Retrieval-Augmented Generation)
Implement knowledge-enhanced agents using Azure AI Foundry's file search capabilities.

**Code Samples:**
- **Python:** [`python-agent-framework-aifoundry-file-search.ipynb`](./05-agentic-rag/code_samples/python-agent-framework-aifoundry-file-search.ipynb)
- **.NET:** [`dotnet-agent-framework-aifoundry-file-search.ipynb`](./05-agentic-rag/code_samples/dotnet-agent-framework-aifoundry-file-search.ipynb)

**Supporting Files:**
- [`document.md`](./05-agentic-rag/code_samples/document.md) - Sample document for RAG demonstrations

### 07. Planning and Design
Explore advanced planning capabilities and design patterns with GitHub Models integration.

**Code Samples:**
- **Python:** [`python-agent-framrwork-ghmodel-planningdesign.ipynb`](./07-planning-design/code_samples/python-agent-framrwork-ghmodel-planningdesign.ipynb)
- **.NET:** [`dotnet-agent-framework-ghmodel-planningdesign`](./07-planning-design/code_samples/dotnet-agent-framework-ghmodel-planningdesign/)

### 08. Multi-Agent Systems
Build collaborative multi-agent workflows using GitHub Models for complex problem-solving scenarios.

**Code Samples:**
- **Python:** [`python-agent-framework-ghmodel-workflow-multi-agents.ipynb`](./08-multi-agent/code_samples/python-agent-framework-ghmodel-workflow-multi-agents.ipynb)
- **.NET:** [`dotnet-agent-framework-ghmodel-workflow-multi-agents.ipynb`](./08-multi-agent/code_samples/dotnet-agent-framework-ghmodel-workflow-multi-agents.ipynb)

### 09. Metacognition
*Coming Soon* - Advanced metacognitive capabilities for self-aware agents.

### 10. AI Agents in Production
*Coming Soon* - Best practices for deploying and managing agents in production environments.

### 11. Agentic Protocols
*Coming Soon* - Advanced communication protocols and standards for agent interactions.

### 12. Context Engineering
*Coming Soon* - Advanced techniques for context management and optimization.

## 🛠 Prerequisites

### Development Environment
- **Python:** Python 3.10 or higher
- **.NET:** .NET 9.0 or higher
- Visual Studio Code with appropriate extensions

### Required Services
- **Azure AI Foundry:** For RAG examples and advanced capabilities
- **GitHub Models:** For GitHub-integrated examples
- **Azure OpenAI Service:** For certain provider examples

### Environment Configuration

Create a `.env` file or set environment variables for the examples:

```env
# GitHub Models Configuration
GITHUB_TOKEN=your_github_token
GITHUB_ENDPOINT=https://models.inference.ai.azure.com
GITHUB_MODEL_ID=gpt-4o-mini

# Azure AI Foundry Configuration
FOUNDRY_PROJECT_ENDPOINT=your_foundry_endpoint
FOUNDRY_MODEL_DEPLOYMENT_NAME=your_model_name

# Azure OpenAI Configuration (if needed)
AZURE_OPENAI_ENDPOINT=your_aoai_endpoint
AZURE_OPENAI_API_KEY=your_aoai_key
AZURE_OPENAI_DEPLOYMENT_NAME=your_deployment_name
```

## 🚀 Getting Started

1. **Choose your preferred language:** Python or .NET
2. **Start with lesson 01:** Introduction to AI Agents
3. **Follow the sequential path:** Each lesson builds upon previous concepts
4. **Experiment with the code:** Modify examples to understand the framework better
5. **Apply learnings:** Use the patterns in your own agent projects

## 📝 Code Sample Structure

Each code sample is provided as a Jupyter notebook (`.ipynb`) containing:
- **Detailed explanations** of concepts and implementation
- **Step-by-step code** with comments and documentation
- **Practical examples** you can run and modify
- **Best practices** for Microsoft Agent Framework usage

## 🔗 Related Resources

- [Microsoft AI Agents for Beginners](https://github.com/microsoft/ai-agents-for-beginners) - Foundational curriculum
- [Microsoft Agent Framework](https://github.com/microsoft/agent-framework) - Official framework repository
- [Azure AI Foundry](https://azure.microsoft.com/en-us/products/ai-foundry) - AI development platform
- [GitHub Models](https://github.com/marketplace/models) - GitHub's AI model marketplace

## 🤝 Contributing

We welcome contributions to improve these examples:
- Report issues or bugs
- Suggest new examples or improvements
- Submit pull requests with enhancements
- Share your own agent implementations

---

**Ready to build intelligent agents?** Start with [lesson 01](./01-intro-to-ai-agents/) and begin your Microsoft Agent Framework journey! 🚀