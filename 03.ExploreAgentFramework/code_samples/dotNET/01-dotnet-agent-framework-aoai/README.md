# Agent Framework — Azure OpenAI Provider (.NET)

Shows how to create an `AIAgent` using **Azure OpenAI** as the backing provider, authenticating with `AzureCliCredential`.

## What it shows
- Constructing an `AIAgent` from `AzureOpenAIClient` (no OpenAI API key required)
- Passwordless authentication via Azure CLI (`az login`)
- Standard and streaming responses against an Azure-hosted deployment

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- An Azure OpenAI resource with a deployed model
- Azure CLI logged in (`az login`)

## Configure secrets

```bash
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<https://your-resource.openai.azure.com/>"
dotnet user-secrets set "AZURE_OPENAI_RESPONSES_DEPLOYMENT_NAME" "gpt-4.1-mini"
```

## Run

```bash
dotnet run
```
