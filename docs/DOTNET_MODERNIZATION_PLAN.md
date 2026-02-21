# .NET Modernization Plan

> Generated: 2026-02-21  
> Scope: All .NET projects in this repository  
> No changes have been made — this is the plan only.

---

## Executive Summary

27 of 28 .NET projects **cannot build today** because they contain `<ProjectReference>` elements pointing to hardcoded absolute paths on the original author's macOS machine (`/Users/lokinfey/Desktop/…`). The one exception (`CreateWorkflowWithYAML`) uses NuGet packages but at severely outdated versions from November 2024.

Additional systemic issues:
- Every project uses `DotNetEnv` + a `.env` file for secrets — not idiomatic .NET and a security risk in repos.
- No root solution file exists to build all projects together.
- Package versions are inconsistent and mix stable/preview incorrectly.
- Several csproj files use `<PackageVersion>` (a Central Package Management declaration element) instead of `<PackageReference>`, which means those Azure SDK packages are silently not installed.
- Folder and project naming has typos and inconsistent casing (`dotNET` vs `dotnet`, `ExploerAgentFramework`, `framrwork`).

---

## Inventory of All .NET Projects

| # | Project Path | Provider | Broken (source refs) | `PackageVersion` bug | Notes |
|---|---|---|---|---|---|
| 1 | `00.ForBeginners/01-intro-to-ai-agents/code_samples/dotnet-agent-framework-travelagent/` | GitHub Models | ✅ | | |
| 2 | `00.ForBeginners/02-explore-agentic-frameworks/code_samples/dotnet-agent-framework-basicagent/` | GitHub Models | ✅ | | |
| 3 | `00.ForBeginners/03-agentic-design-patterns/code_samples/dotnet-agent-framework-basicagent/` | GitHub Models | ✅ | | |
| 4 | `00.ForBeginners/04-tool-use/code_samples/dotnet-agent-framework-ghmodels-tool/` | GitHub Models | ✅ | | |
| 5 | `00.ForBeginners/05-agentic-rag/code_samples/dotnet-agent-framework-msfoundry-file-search/` | MS Foundry | ✅ | ✅ | `<PackageVersion>` on Azure.AI.Projects, Azure.AI.Agents.Persistent |
| 6 | `00.ForBeginners/07-planning-design/code_samples/dotnet-agent-framrwork-ghmodel-planningdesign/` | GitHub Models | ✅ | | Folder name typo: `framrwork` |
| 7 | `00.ForBeginners/08-multi-agent/code_samples/dotnet-agent-framework-ghmodel-workflow-multi-agents/` | GitHub Models | ✅ | | |
| 8 | `02.CreateYourFirstAgent/code_samples/dotNET/dotnet-travelagent-ghmodel/` | GitHub Models | ✅ | | |
| 9 | `03.ExploerAgentFramework/code_samples/dotNET/01-dotnet-agent-framework-aoai/` | Azure OpenAI | ✅ | | Chapter folder name typo: `ExploerAgentFramework` |
| 10 | `03.ExploerAgentFramework/code_samples/dotNET/02-dotnet-agent-framework-ghmodel/` | GitHub Models | ✅ | | |
| 11 | `03.ExploerAgentFramework/code_samples/dotNET/03-dotnet-agent-framework-msfoundry/` | MS Foundry | ✅ | | Also has hardcoded macOS `.env` path in `Program.cs` |
| 12 | `03.ExploerAgentFramework/code_samples/dotNET/04-dotnet-agent-framework-foundrylocal/` | Foundry Local | ✅ | | |
| 13 | `04.Tools/code_samples/dotNET/msfoundry/01-dotnet-agent-framework-msfoundry-vision/` | MS Foundry | ✅ | ✅ | `<PackageVersion>` on Azure.AI.Projects, Azure.AI.Agents.Persistent |
| 14 | `04.Tools/code_samples/dotNET/msfoundry/02-dotnet-agent-framework-msfoundry-code-interpreter/` | MS Foundry | ✅ | | |
| 15 | `04.Tools/code_samples/dotNET/msfoundry/03-dotnet-agent-framework-msfoundry-binggrounding/` | MS Foundry | ✅ | | |
| 16 | `04.Tools/code_samples/dotNET/msfoundry/04-dotnet-agent-framework-msfoundry-file-search/` | MS Foundry | ✅ | | |
| 17 | `05.Providers/code_samples/dotNET/01-dotnet-agent-framework-aifoundry-mcp/AgentMCP.Console/` | MS Foundry + MCP | ✅ | | |
| 18 | `06.RAGs/code_samples/dotNET/dotnet-agent-framework-msfoundry-file-search/` | MS Foundry | ✅ | ✅ | `<PackageVersion>` on Azure.AI.Projects, Azure.AI.Agents.Persistent |
| 19 | `07.Workflow/code_samples/dotNET/01.dotnet-agent-framework-workflow-ghmodel-basic/` | GitHub Models | ✅ | | |
| 20 | `07.Workflow/code_samples/dotNET/02.dotnet-agent-framework-workflow-ghmodel-sequential/` | GitHub Models | ✅ | | |
| 21 | `07.Workflow/code_samples/dotNET/03.dotnet-agent-framework-workflow-ghmodel-concurrent/` | GitHub Models | ✅ | | |
| 22 | `07.Workflow/code_samples/dotNET/04.dotnet-agent-framework-workflow-msfoundry-condition/` | MS Foundry | ✅ | | |
| 23 | `08.EvaluationAndTracing/dotNET/GHModel.dotNET.AI.Workflow.DevUI/` | GitHub Models | ✅ | | ASP.NET Core web app |
| 24 | `09.Cases/MicrosoftFoundryWithAITKAndMAF/CreateWorkflowWithYAML/` | MS Foundry | ❌ (uses NuGet) | | **Only project using NuGet** but packages are `1.0.0-preview.251114.1` (Nov 2024) |
| 25 | `09.Cases/GHModel.AI/GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.DevUI/` | GitHub Models | ✅ | | ASP.NET Core web app |
| 26 | `09.Cases/GHModel.AI/GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.OpenTelemetry/` | GitHub Models | ✅ | | OpenTelemetry tracing |
| 27 | `09.Cases/GHModel.AI/GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.AGUI/GHModel.dotNET.AI.Workflow.AGUI.Server/` | GitHub Models | ✅ | | ASP.NET Core AGUI server |
| 28 | `09.Cases/GHModel.AI/GHModel.dotNET.AI/GHModel.dotNET.AI.Workflow.AGUI/GHModel.dotNET.AI.Workflow.AGUI.Client/` | GitHub Models | ✅ | | Blazor AGUI client |

**27/28 projects are completely broken. 1/28 builds but uses outdated packages.**

---

## Source-Project-to-NuGet Mapping

Every broken `<ProjectReference>` maps to a published NuGet package. The plan is to replace all of them:

| Source project (current broken reference) | NuGet package name |
|---|---|
| `Microsoft.Agents.AI` | `Microsoft.Agents.AI` |
| `Microsoft.Agents.AI.OpenAI` | `Microsoft.Agents.AI.OpenAI` |
| `Microsoft.Agents.AI.AzureAI` | `Microsoft.Agents.AI.AzureAI` |
| `Microsoft.Agents.AI.AzureAI.Persistent` | `Microsoft.Agents.AI.AzureAI.Persistent` |
| `Microsoft.Agents.AI.Workflows` | `Microsoft.Agents.AI.Workflows` |
| `Microsoft.Agents.AI.DevUI` | `Microsoft.Agents.AI.DevUI` |
| `Microsoft.Agents.AI.Hosting` | `Microsoft.Agents.AI.Hosting` |
| `Microsoft.Agents.AI.Hosting.OpenAI` | `Microsoft.Agents.AI.Hosting.OpenAI` |
| `Microsoft.Agents.AI.AGUI` | `Microsoft.Agents.AI.AGUI` |
| `Microsoft.Agents.AI.Hosting.AGUI.AspNetCore` | `Microsoft.Agents.AI.Hosting.AGUI.AspNetCore` |

**Before implementing:** run `dotnet package search Microsoft.Agents.AI --prerelease` to get the current latest version for each package. At plan-writing time the only recorded NuGet version is `1.0.0-preview.251114.1`; use the latest available at implementation time.

---

## Phase 1 — Create Root Solution

### Action
Create `dotnet/AgentFrameworkSamples.sln` (or `.slnx`) at the repo root level under a new `dotnet/` folder.

### Steps
1. `mkdir dotnet`
2. `cd dotnet && dotnet new sln -n AgentFrameworkSamples`
3. Add all 28 projects to the solution via `dotnet sln add <relative-path-to.csproj>`.

### Why
- A single solution is the standard way to work with multi-project .NET repos.
- Enables `dotnet build dotnet/AgentFrameworkSamples.sln` to verify the entire tree at once.
- A `.slnx` (the new XML-based solution format) is preferred for .NET 10 — use `dotnet new slnx` if available.

### Note on project location
The projects **stay in their current chapter folders**. The solution file in `dotnet/` just references them. This preserves the educational chapter structure while adding root-level build support.

---

## Phase 2 — NuGet Package Modernization (all 28 projects)

### Action
In every `.csproj`:
1. Remove all `<ProjectReference>` elements pointing to `/Users/lokinfey/…`.
2. Add the corresponding `<PackageReference>` elements with the latest versions of `Microsoft.Agents.AI.*` NuGet packages.
3. Fix the 3 projects that use `<PackageVersion>` (wrong element) — change to `<PackageReference>`.
4. Remove the `DotNetEnv` package reference from every project.
5. Pin all non-Agent-Framework packages to their latest stable versions at implementation time.

### Packages to upgrade across the board
Cross-cutting packages that appear in all or most projects and should be updated to latest:

| Package | Current version in repo | Action |
|---|---|---|
| `Azure.AI.OpenAI` | `2.1.0` | Update to latest stable |
| `Azure.Identity` | `1.17.1` / `1.18.0-beta.2` | Update to latest stable |
| `Azure.AI.Projects` | `1.2.0-beta.5` | Update to latest available |
| `Azure.AI.Projects.OpenAI` | `1.0.0-beta.5` | Update to latest available |
| `Azure.AI.Agents.Persistent` | `1.2.0-beta.8` | Update to latest available |
| `Microsoft.Extensions.AI` | `10.2.0` | Update to latest |
| `Microsoft.Extensions.AI.OpenAI` | `10.2.0-preview.1.26063.2` | Update to latest |
| `OpenAI` | `2.8.0` | Update to latest stable |
| `ModelContextProtocol` | `0.6.0-preview.1` | Update to latest available |
| `OpenTelemetry` / extensions | `1.13.x` | Update to latest stable |
| `DotNetEnv` | `3.1.1` | **REMOVE** (replaced by user secrets) |

### `PackageVersion` bug fix (3 projects)
Projects `#5`, `#13`, `#18` in the inventory use `<PackageVersion>` inside an `<ItemGroup>` without a `<PackageReference>`. This is a symptom of Central Package Management syntax used outside a `Directory.Packages.props` context — the packages are declared but never actually installed. Change each occurrence to `<PackageReference>`.

---

## Phase 3 — Replace `.env` with User Secrets

### Problem
All projects use `DotNetEnv` to load a `.env` file at a relative path (`../../../../.env`) or an absolute macOS path. This pattern:
- Only works when run from a specific directory.
- Puts credentials in a plain text file that can easily be committed to git.
- Is not idiomatic .NET.

### Solution
Replace with `Microsoft.Extensions.Configuration.UserSecrets` for local development. This stores secrets in the OS user profile, outside the repo.

### Changes per project

**In each `.csproj`:**
```xml
<PropertyGroup>
  <UserSecretsId><!-- use a fresh GUID per project: dotnet-new-guid --></UserSecretsId>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.0" />
  <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="10.0.0" />
</ItemGroup>
```

**In each `Program.cs`:**
Replace:
```csharp
using DotNetEnv;
Env.Load("../../../../.env");
var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw ...;
```

With:
```csharp
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()   // allows CI/CD override without code change
    .Build();

var token = config["GITHUB_TOKEN"] ?? throw new InvalidOperationException("GITHUB_TOKEN not set. Run: dotnet user-secrets set GITHUB_TOKEN <value>");
```

The `AddEnvironmentVariables()` call ensures environment variables still work as a fallback (useful in CI/CD).

### Per-project secret key sets

**GitHub Models projects** (projects #1–8, 10, 19–23, 25–28):
```
GITHUB_TOKEN
GITHUB_ENDPOINT
GITHUB_MODEL_ID
```

**Azure OpenAI projects** (project #9):
```
AZURE_OPENAI_ENDPOINT
AZURE_OPENAI_CHAT_DEPLOYMENT_NAME
```

**MS Foundry projects** (projects #5, 11, 13–18, 22, 24):
```
AZURE_AI_PROJECT_ENDPOINT
AZURE_AI_MODEL_DEPLOYMENT_NAME
```

**MS Foundry Bing Grounding projects** (project #15, 22):
```
BING_CONNECTION_ID
BING_CONNECTION_NAME
```

**Foundry Local projects** (project #12):
```
FOUNDRYLOCAL_ENDPOINT
FOUNDRYLOCAL_MODEL_DEPLOYMENT_NAME
```

**OpenTelemetry projects** (project #26):
```
OTEL_EXPORTER_OTLP_ENDPOINT
```

### User secrets setup commands (to document in each README)
```bash
# Run from the project folder
dotnet user-secrets set "GITHUB_TOKEN" "your-token-here"
dotnet user-secrets set "GITHUB_ENDPOINT" "https://models.inference.ai.azure.com"
dotnet user-secrets set "GITHUB_MODEL_ID" "gpt-4o-mini"
```

---

## Phase 4 — Per-Project README Files

Each sample directory needs a `README.md` with the same minimal structure:

```
# <Sample Name>

One-line description of what this sample demonstrates.

## What it shows
- Bullet: key Agent Framework concept demonstrated

## Prerequisites
- .NET 10 SDK
- [Provider-specific prerequisite, e.g., GitHub Models token]

## Configure secrets
\`\`\`bash
dotnet user-secrets set "KEY" "value"
\`\`\`

## Run
\`\`\`bash
dotnet run
\`\`\`
```

Keep READMEs minimal and focused on the Agent Framework concept being demonstrated. Do not repeat generic .NET or Azure setup instructions — link to the root README for those.

### Projects that currently have no `README.md`
Based on the audit, most individual sample projects have no README at all. All 28 projects need one created.

---

## Phase 5 — `dotnet run` Compatibility

### Constraint
All console apps must be runnable with `dotnet run` from the project folder (no extra tooling, no IDE required).

### Review each project type:

**Console apps (SDK: `Microsoft.NET.Sdk`, `OutputType: Exe`)** — 25 projects  
These already support `dotnet run` once the build is fixed. No structural changes needed.

**ASP.NET Core web apps (SDK: `Microsoft.NET.Sdk.Web`)** — 4 projects  
- `08.EvaluationAndTracing/dotNET/GHModel.dotNET.AI.Workflow.DevUI/`
- `09.Cases/GHModel.AI/.../GHModel.dotNET.AI.Workflow.DevUI/`
- `09.Cases/GHModel.AI/.../GHModel.dotNET.AI.Workflow.AGUI.Server/`
- `09.Cases/GHModel.AI/.../GHModel.dotNET.AI.Workflow.AGUI.Client/`

These support `dotnet run` natively — the README should document the URL printed at startup (e.g., `http://localhost:50518/devui`).

---

## Phase 6 — Root README Updates

### Table status column
Add a status badge or indicator (✅ Working / ⚠️ Needs setup / ❌ Broken) to the repository structure table for each .NET sample. After completing the preceding phases, all should be ✅.

### Prerequisites section
The current Prerequisites section says:  
> *"we strongly recommend building from source rather than using NuGet packages"*

Replace this with:
> *"Install .NET 10 SDK. All .NET samples use NuGet packages and run with `dotnet run`."*

Remove the "Build from Source" blocks for .NET entirely. The Python section is out of scope.

### `.env` Quick Start section
Remove the `Create a .env file` section from the root README (it applies to Python only after this change). Replace the .NET part with a pointer to `dotnet user-secrets`.

---

## Phase 7 — Naming and Structure Cleanup

These changes have low risk of breaking links if handled carefully:

| Issue | Current | Proposed fix |
|---|---|---|
| Chapter folder typo | `03.ExploerAgentFramework` | Rename to `03.ExploreAgentFramework` (update all internal links) |
| Project folder typo | `dotnet-agent-framrwork-ghmodel-planningdesign` | Rename to `dotnet-agent-framework-ghmodel-planningdesign` |
| Inconsistent casing on provider subfolders | `dotNET` | Standardize to `dotnet` |
| Hardcoded macOS path in `Program.cs` | `Env.Load("/Users/lokinfey/Desktop/…")` | Removed by Phase 3 |

**Note:** Renaming `03.ExploerAgentFramework` → `03.ExploreAgentFramework` requires updating all links in the root `README.md` and any cross-reference READMEs. Search for `ExploerAgentFramework` before applying.

---

## Phase 8 — Additional Improvements

### 8.1 Add a `Directory.Build.props` at repo root (under `dotnet/`)
Centralizes shared properties so they don't need to be repeated in every `.csproj`:

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <NoWarn>$(NoWarn);OPENAI001;MEAI001</NoWarn>
  </PropertyGroup>
</Project>
```

This avoids duplicating `<TargetFramework>net10.0</TargetFramework>` and warning suppressions across 28 files.

### 8.2 Add a `global.json` at repo root
Pins the .NET SDK version so all contributors use the same toolchain:

```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestMinor"
  }
}
```

### 8.3 Add `.gitignore` entries for user secrets if missing
User secrets are stored outside the repo by the SDK, but add a comment in `.gitignore` to explain this to contributors. Also confirm `*.env` and `.env` are already ignored (they are, based on the existing `.env.examples` pattern).

### 8.4 Consolidate duplicate samples
Samples `#1` and `#8` are functionally identical travel agent demos (one in `00.ForBeginners`, one in `02.CreateYourFirstAgent`). Consider whether both are needed or if they should diverge conceptually. This is a content decision, not a technical one — flag it for the maintainer.

Similarly, `#18` (RAG in `06.RAGs`) and `#5` (RAG in `00.ForBeginners/05`) appear to be the same sample. Verify whether both are intentional.

### 8.5 Remove dead commented-out code
Several `Program.cs` files contain large blocks of commented-out code (e.g., `07.Workflow/04`, `09.Cases/GHModel.AI/...`). Clean these up — the git history provides rollback if needed.

### 8.6 DevUI projects: document the port
The DevUI and AGUI projects start an ASP.NET web server. The port (`50518`) is only visible in a `Console.WriteLine`. Add a `launchSettings.json` with an explicit port and document it in the README so contributors know where to navigate.

### 8.7 OpenTelemetry project: verify Aspire dashboard integration
`09.Cases/GHModel.AI/.../GHModel.dotNET.AI.Workflow.OpenTelemetry/` exports traces via OTLP. Add a note in its README about using the .NET Aspire dashboard as a local collector (`dotnet run --project` of the Aspire dashboard) as the easiest local tracing experience.

---

## Execution Order

The phases should be executed in this order to minimize rework:

1. **Phase 1** — Create root solution (unblocks `dotnet build` verification at each step)
2. **Phase 2** — NuGet package modernization (fixes the broken builds)
3. **Phase 3** — Replace `.env` with user secrets (removes `DotNetEnv` dependency)
4. **Phase 5** — Verify `dotnet run` for all projects (smoke test after 2+3)
5. **Phase 4** — Write per-project READMEs (easier once the code is working)
6. **Phase 7** — Naming cleanup (do last to reduce link-churn during earlier phases)
7. **Phase 6** — Root README updates (update links after naming is finalized)
8. **Phase 8** — Additional improvements (non-blocking enhancements)

---

## Effort Estimate by Project Count

| Phase | Projects affected | Notes |
|---|---|---|
| Phase 1 | 1 new file | ~5 minutes |
| Phase 2 | 28 `.csproj` files | Mechanical, scriptable |
| Phase 3 | 28 `Program.cs` files | Mechanical, varies by secret count |
| Phase 4 | 28 new `README.md` files | Content writing |
| Phase 5 | 4 web projects | Verify port/launch |
| Phase 6 | 1 `README.md` | Medium |
| Phase 7 | ~50 file renames + link updates | Grep for `ExploerAgentFramework` first |
| Phase 8 | 2 new files + 28 csproj | Low risk |

---

## Out of Scope

- Any Python, Jupyter Notebook, or non-.NET content
- `09.Cases/AgenticMarketingContentGen/` — Python only
- `09.Cases/FoundryLocalPipeline/` — Python only
- `09.Cases/MicrosoftFoundryWithAITKAndMAF/YAML/` — YAML config (no .NET code)
- `08.EvaluationAndTracing/python/` — Python only
- Root README sections covering Python prerequisites, Python quick start
