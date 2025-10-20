# Change Log - Agent Framework Samples

## 2025-10-20

### Repository Documentation Updates

**Summary:** Updated main README.md to accurately reflect project structure and added missing EvaluationAndTracing section

#### Documentation Improvements

| Component | Changes Made |
|-----------|--------------|
| `README.md` | • Updated Repository Structure table to include missing `08.EvaluationAndTracing` directory<br>• Added comprehensive documentation for evaluation and debugging tools<br>• Updated Tutorial Progression to include advanced evaluation techniques<br>• Enhanced Key Features section with evaluation and tracing capabilities<br>• Improved project navigation with accurate directory references |

#### Repository Structure Alignment

| Section | Updates |
|---------|---------|
| Repository Structure Table | • Added `08.EvaluationAndTracing` with Python-only samples<br>• Documented DevUI visualization tools<br>• Listed observability and tracing examples<br>• Maintained consistency with actual project structure |
| Tutorial Progression | • Added evaluation and tracing as advanced level topic<br>• Organized learning path from beginner to advanced concepts<br>• Enhanced developer journey documentation |
| Key Features | • Documented DevUI for visual debugging<br>• Added observability tools for agent tracing<br>• Highlighted evaluation capabilities for agent development |

## 2025-10-08

### Added Evaluation and Tracing Components

**Summary:** Introduced comprehensive evaluation and tracing capabilities for agent development with DevUI visualization and observability features

#### Sample Code and Documentation

***Folder*** - *08.EvaluationAndTracing*

| Component | Content Added |
|-----------|---------------|
| `README.md` | • Comprehensive tutorial on evaluation and tracing<br>• DevUI setup and usage instructions<br>• Observability configuration guide<br>• Key features and benefits documentation |

***Folder*** - *08.EvaluationAndTracing/python*

| Sample | Implementation |
|--------|----------------|
| `basic_agent_workflow_devui/` | • Basic agent workflow with DevUI integration<br>• Real-time session visualization example<br>• Interactive debugging demonstration |
| `multi_workflow_ghmodel_devui/` | • Multi-agent workflow with DevUI support<br>• Complex interaction pattern visualization<br>• GitHub Models integration with monitoring |
| `tracer_aspire/` | • Observability and logging configuration<br>• Step-by-step execution tracing<br>• Python logging framework integration |

## 2025-10-07


#### Python Samples Update

***Folder*** - *00.ForBeginners/05-agentic-rag/code_samples*

| File | Changes Made |
|------|--------------|
| `python-agent-framework-aifoundry-file-search.ipynb` | • Updated Azure AI Foundry file search integration<br>• Enhanced document processing with vector store management<br>• Improved RAG capabilities with `HostedFileSearchTool`<br>• Updated agent creation with persistent file search tools |

***Folder*** - *03.ExploerAgentFramework/code_samples/python*

| File | Changes Made |
|------|--------------|
| `03-python-agent-framework-aifoundry.ipynb` | • Updated Azure AI Foundry basic agent integration<br>• Enhanced agent lifecycle management<br>• Improved `AzureAIAgentClient` initialization patterns<br>• Updated async context management for agents |

***Folder*** - *04.Tools/code_samples/python/foundry*

| File | Changes Made |
|------|--------------|
| `01.python-agent-framework-aifoundry-vision.ipynb` | • Updated Azure AI Foundry vision capabilities<br>• Enhanced image processing with base64 encoding<br>• Improved multimodal content handling<br>• Updated vision agent creation with furniture consultation features |
| `02.python-agent-framework-aifoundry-code-interpreter.ipynb` | • Updated Azure AI Foundry code interpreter integration<br>• Enhanced code execution capabilities<br>• Improved `HostedCodeInterpreterTool` implementation<br>• Updated mathematical computation features |
| `03.python-agent-framework-aifoundry-binggrounding.ipynb` | • Updated Azure AI Foundry Bing grounding integration<br>• Enhanced web search capabilities<br>• Improved `HostedWebSearchTool` implementation<br>• Updated connection-based search functionality |
| `04.python-agent-framework-aifoundry-file-search.ipynb` | • Updated Azure AI Foundry advanced file search<br>• Enhanced vector store creation and management<br>• Improved document upload and processing workflows<br>• Updated streaming response capabilities for file search |

## 2025-10-03

### Fixed GraphViz Integration

**Summary:** Added GraphViz installation and visualization capabilities to all workflow samples

#### A. DevContainer Update

| Component | Change Description |
|-----------|-------------------|
| Development Environment | Added GraphViz system package to devcontainer configuration |
| Installation | Pre-configured GraphViz to eliminate manual setup steps |

#### B. Workflow Samples Update

***Folder*** - *07.Workflow/code_samples/python*

| File | Changes Made |
|------|--------------|
| `01.python-agent-framework-workflow-ghmodel-basic.ipynb` | • Added `sudo apt install graphviz -y` installation<br>• Integrated WorkflowViz for workflow visualization<br>• Added SVG export with `viz.export(format="svg")`<br>• Implemented inline notebook rendering with HTML fallback |
| `02.python-agent-framework-workflow-ghmodel-sequential.ipynb` | • Added GraphViz system installation commands<br>• Integrated workflow visualization for sequential pipeline<br>• Added SVG generation for three-agent workflow<br>• Implemented consistent visualization pattern |
| `03.python-agent-framework-workflow-ghmodel-concurrent.ipynb` | • Added GraphViz installation for concurrent workflows<br>• Implemented parallel execution diagram generation<br>• Added visual representation of concurrent agents<br>• Consistent SVG export functionality |
| `04.python-agent-framework-workflow-aifoundry-condition.ipynb` | • Added GraphViz support for conditional workflows<br>• Implemented decision tree diagram generation<br>• Added conditional branching visualization<br>• Consistent visualization approach |