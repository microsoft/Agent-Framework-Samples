# How to Use an Microsoft Foundry Local to Build a Deep Research Scenario

This repo shows how to use a Microsoft Foundry Local (example: Qwen2.5-1.5B) to build a Deep Research workflow, and how to evaluate, orchestrate, and visualize it with Microsoft Agent Framework (MAF).

## What You Will See

- Foundry Local provides a local AI runtime for running SLMs on your own machine. 
- Microsoft Agent Framework support Foundry Local
- Red teaming evaluation with MAF for safety and compliance risk checks.
- An iterative Deep Research workflow built with MAF Workflows (with a web search tool).
- Interactive observation and debugging with MAF DevUI.
- A path toward observability and performance evaluation (with .NET Aspire).


## Scenario Path (Step by Step)


1. Foundry Local
   - Run the SLM offline or in constrained environments.
   - Keep data and prompts on-device for privacy-sensitive workflows.
   - Develop and iterate faster without remote inference latency.

    Learn more: https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-local/what-is-foundry-local

2. Evaluate an SLM with Microsoft Agent Framework
   - Use red teaming to probe risk categories such as violence, hate/unfairness, sexual content, and self-harm.
   - Run multiple attack strategies (e.g., encoding, obfuscation, character swaps) to test robustness.
   - Review scorecards and outputs to understand safety gaps and response behavior.
   - Repo script:
     - [01.foundrylocal_maf_evaluation.py](01.foundrylocal_maf_evaluation.py)

3. Integrate Deep Research with Microsoft Agent Framework Workflows
   - Build a looped workflow that alternates between research and control decisions.
   - Use a tool-enabled research agent to run web search, summarize, and identify knowledge gaps.
   - Produce a final report that synthesizes all iterations with citations.
   - Repo script:
     - [02.foundrylocal_maf_workflow_deep_research_devui.py](02.foundrylocal_maf_workflow_deep_research_devui.py)

4. Use Microsoft Agent Framework DevUI to evaluate Deep Research
   - Launch DevUI to visualize the workflow graph and step-by-step execution.
   - Inject new topics and observe iterative states, agent responses, and final report output.
   - Validate loop behavior, tool calls, and output quality interactively.
   - Repo script (DevUI enabled by default):
     - [02.foundrylocal_maf_workflow_deep_research_devui.py](02.foundrylocal_maf_workflow_deep_research_devui.py)

5. Use Microsoft Agent Framework and .NET Aspire for performance evaluation
   - Enable tracing and metrics to analyze latency, throughput, and tool usage.
   - Correlate workflow stages with system telemetry to spot bottlenecks.
   - Use distributed tracing to understand cross-component performance in agent runs.
     - [02.foundrylocal_maf_workflow_deep_research_devui.py](02.foundrylocal_maf_workflow_deep_research_devui.py)

## Quick Start

### 1) Environment Variables

This repo reads configuration from environment variables:

- `FOUNDRYLOCAL_ENDPOINT`: Foundry Local endpoint
- `FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME`: local model deployment name (example: `qwen2.5-1.5b-instruct-generic-cpu:4`)
- `SERPAPI_API_KEY`: for web search (SerpAPI)
- `AZURE_AI_PROJECT_ENDPOINT`: Azure AI project endpoint (for red teaming evaluation)
- `OTLP_ENDPOINT`: OpenTelemetry endpoint for traces/metrics (optional)

Example (placeholder values):

```bash
export FOUNDRYLOCAL_ENDPOINT="<your_foundry_local_endpoint>"
export FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME="qwen2.5-1.5b-instruct-generic-cpu:4"
export SERPAPI_API_KEY="<your_serpapi_key>"
export AZURE_AI_PROJECT_ENDPOINT="<your_azure_ai_project_endpoint>"
export OTLP_ENDPOINT="http://localhost:4317"
```

If you want to use Azure evaluation capabilities, make sure you have run `az login`.

### 2) Run Red Teaming

```bash
python 01.foundrylocal_maf_evaluation.py
```

Results are written to: [Qwen2.5-1.5B-Redteam-Results.json](Qwen2.5-1.5B-Redteam-Results.json).

### 3) Run Deep Research (DevUI Mode)

```bash
python 02.foundrylocal_maf_workflow_deep_research_devui.py
```

DevUI starts at `http://localhost:8093`. Enter a topic to see iterative research and the final report.

### 4) Run Deep Research (CLI Mode)

```bash
python 02.foundrylocal_maf_workflow_deep_research_devui.py --cli
```

## Code Map

- Red teaming entry: [01.foundrylocal_maf_evaluation.py](01.foundrylocal_maf_evaluation.py)
- Deep Research workflow + DevUI: [02.foundrylocal_maf_workflow_deep_research_devui.py](02.foundrylocal_maf_workflow_deep_research_devui.py)
- Web search helper: [utils.py](utils.py)

## Notes

- The workflow uses the `search_web` tool by default and outputs iterative summaries plus a final report.
- The default model is `qwen2.5-1.5b-instruct-generic-cpu:4`. Update the scripts if you want to use a different local model.

## Deep Research Workflow Structure

![workflow](./imgs/workflow.png)