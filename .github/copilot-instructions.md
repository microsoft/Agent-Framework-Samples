# GitHub Copilot Instructions

## Repository Layout Rules

### Root-level files
The root of this repository must contain **only**:
- `README.md` — the main repository readme
- `LICENSE` — the license file

All other documentation, planning documents, and policy files must be placed in the `docs/` folder at the root of the repository.

### docs/ folder
The `docs/` folder at the root of the repository is the single location for:
- Planning documents (e.g., modernization plans, roadmaps)
- Policy files (e.g., CODE_OF_CONDUCT.md, SECURITY.md, SUPPORT.md)
- Any other repository-wide documentation that is not a `README.md` or `LICENSE`

**This rule does NOT apply to documentation that belongs to individual samples.**  
Each sample folder (e.g., `02.CreateYourFirstAgent/`, `07.Workflow/`, `09.Cases/`) may contain its own `README.md` and any documentation files specific to that sample.

### Sample folders
Each numbered chapter folder and its sub-folders may freely contain:
- `README.md`
- Sample-specific documentation and assets (images, diagrams, etc.)
- Code files and project files

### Summary

| Location | Allowed files |
|---|---|
| Repo root | `README.md`, `LICENSE` only |
| `docs/` | All repo-wide docs, plans, and policy files |
| Any sample/chapter folder | `README.md`, sample-specific docs, code |

When creating new planning documents, policy files, or repository-wide documentation, always place them in `docs/` — never at the repo root.
