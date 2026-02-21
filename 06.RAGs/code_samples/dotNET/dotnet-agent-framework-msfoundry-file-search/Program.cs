// ============================================================
//  RETRIEVAL-AUGMENTED GENERATION (RAG) PIPELINE
//
//  RAG grounds an LLM in your own documents so it can answer
//  questions about private data it was not trained on.
//
//  This sample walks through all five steps:
//
//    Step 1: Upload  — send your document(s) to Azure AI Foundry
//    Step 2: Index   — create a vector store (searchable index)
//    Step 3: Configure — build a HostedFileSearchTool linked to the store
//    Step 4: Create  — define the agent with strict "document-only" instructions
//    Step 5: Query   — run multi-turn Q&A; the agent cites its sources
//
//  Unlike the tool introduction in 04.Tools, the focus here is on
//  building a grounded, citation-accurate assistant that refuses to
//  answer from general knowledge.
// ============================================================

using System.ClientModel;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Files;
using OpenAI.VectorStores;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var endpoint       = config["AZURE_AI_PROJECT_ENDPOINT"]        ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var deploymentName = config["AZURE_AI_MODEL_DEPLOYMENT_NAME"] ?? "gpt-4o-mini";

AIProjectClient aiProjectClient = new(new Uri(endpoint), new AzureCliCredential());
OpenAIClient    openAIClient    = aiProjectClient.GetProjectOpenAIClient();

// -----------------------------------------------
// Step 1 — Upload
// Upload the knowledge document to Azure AI Foundry.
// The file is stored server-side and assigned a unique ID.
// -----------------------------------------------
Console.WriteLine("[Step 1] Uploading knowledge document...");
OpenAIFileClient fileClient   = openAIClient.GetOpenAIFileClient();
var              uploadResult = await fileClient.UploadFileAsync(
    filePath: "../../files/demo.md",
    purpose: FileUploadPurpose.Assistants);
Console.WriteLine($"        File uploaded: {uploadResult.Value.Id}");

// -----------------------------------------------
// Step 2 — Index
// Create a vector store from the uploaded file.
// The vector store converts the document into embeddings so the
// agent can perform semantic (meaning-based) search at query time.
// -----------------------------------------------
Console.WriteLine("[Step 2] Creating vector store (semantic index)...");
#pragma warning disable OPENAI001
VectorStoreClient vectorStoreClient = openAIClient.GetVectorStoreClient();
var               vectorStoreResult = await vectorStoreClient.CreateVectorStoreAsync(
    options: new VectorStoreCreationOptions()
    {
        Name    = "rag-knowledge-base",
        FileIds = { uploadResult.Value.Id }
    });
#pragma warning restore OPENAI001
Console.WriteLine($"        Vector store created: {vectorStoreResult.Value.Id}");

// -----------------------------------------------
// Step 3 — Configure
// Attach the vector store to a HostedFileSearchTool.
// The tool is the bridge between the agent and the index.
// -----------------------------------------------
Console.WriteLine("[Step 3] Configuring HostedFileSearchTool...");
var fileSearchTool = new HostedFileSearchTool()
{
    Inputs = [new HostedVectorStoreContent(vectorStoreResult.Value.Id)]
};

// -----------------------------------------------
// Step 4 — Create the RAG agent
// The system prompt is intentionally strict:
//   • Answer ONLY from the retrieved document context.
//   • Never use general knowledge or make assumptions.
//   • Cite the source document for every factual claim.
//   • Admit clearly when the document does not cover the question.
// This prevents hallucinations and keeps answers grounded.
// -----------------------------------------------
Console.WriteLine("[Step 4] Creating RAG agent...");
AIAgent agent = await aiProjectClient.CreateAIAgentAsync(
    model:        deploymentName,
    name:         "DocumentRAGAgent",
    instructions: """
        You are a document-grounded assistant. Answer user questions using ONLY
        the information retrieved from the provided file(s).

        Rules:
        - Cite the source document for every factual statement.
        - If the document does not contain the answer, respond:
          "The uploaded document does not contain information to answer that question."
        - Never answer from general knowledge, training data, or assumptions.
        - Keep answers concise and accurate.
        """,
    tools:        [fileSearchTool]);

// -----------------------------------------------
// Step 5 — Query (multi-turn)
// An AgentSession holds the conversation history so follow-up
// questions can refer back to earlier answers.
// -----------------------------------------------
Console.WriteLine("[Step 5] Starting multi-turn RAG session...");
Console.WriteLine();

AgentSession session = await agent.CreateSessionAsync();

// Turn 1 — Retrieve a core concept
Console.WriteLine("Q1: What is GraphRAG?");
Console.WriteLine(await agent.RunAsync("What is GraphRAG?", session));
Console.WriteLine();

// Turn 2 — Drill into use cases (tests multi-turn context)
Console.WriteLine("Q2: What are its main use cases?");
Console.WriteLine(await agent.RunAsync("What are its main use cases?", session));
Console.WriteLine();

// Turn 3 — Verify document boundaries (tests refusal on out-of-scope questions)
Console.WriteLine("Q3: How does it compare to Elasticsearch?");
Console.WriteLine(await agent.RunAsync("How does it compare to Elasticsearch?", session));

