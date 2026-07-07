# LLM-as-Judge Agent Evaluation

This sample demonstrates how to evaluate AI agent responses using LLM-as-Judge methodology — a critical production pattern for agentic AI systems.

## What You'll Learn

- Building rubric-based evaluation judges (Correctness, Completeness, Clarity, Safety)
- Creating structured evaluation datasets with expected answers
- Running multiple judges in parallel via `asyncio.gather`
- Aggregating scores and generating evaluation reports
- Integrating evaluation into CI/CD pipelines

## Architecture

```
Test Dataset → Agent Under Test → Multiple Judge Agents → Aggregated Report
                                    ├── Correctness Judge (1-5)
                                    ├── Completeness Judge (1-5)
                                    ├── Clarity Judge (1-5)
                                    └── Safety Judge (1-5)
```

## Prerequisites

```bash
pip install -r requirements.txt
```

## Quick Start

1. Configure `.env` with your model provider credentials
2. Run the notebook cells in sequence
3. View the evaluation report at the end

## Files

| File | Purpose |
|------|---------|
| `llm_judge_evaluation.ipynb` | Main notebook with full evaluation pipeline |
| `datasets/sample_eval_cases.json` | 8 evaluation test cases with expected keywords |
| `requirements.txt` | Python dependencies |
| `.env.example` | Environment variable template |

## Production Integration

This evaluation pattern can be integrated into CI/CD:
```yaml
# GitHub Actions snippet
- name: Run Agent Evaluations
  run: |
    jupyter nbconvert --to notebook --execute llm_judge_evaluation.ipynb
    if grep -q '"overall_score": "[1-3]"' eval-report.json; then exit 1; fi
```
