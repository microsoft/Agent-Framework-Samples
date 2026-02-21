# AGUI Client — AG-UI Protocol (.NET)

An interactive console client that connects to the companion **AGUI Server** over the **AG-UI protocol** using `AGUIChatClient`. Demonstrates how any AG-UI-compliant server can be consumed as a standard `AIAgent` on the client side.

## What it shows
- Connecting to a remote AG-UI server with `AGUIChatClient`
- Wrapping the remote endpoint as a first-class `AIAgent` via `AsAIAgent()`
- Multi-turn streaming conversation loop using `RunStreamingAsync`

## Prerequisites
- [.NET 10 SDK](https://dot.net)
- The companion [AGUI Server](../GHModel.dotNET.AI.Workflow.AGUI.Server/) running at `http://localhost:5018`

## Configure secrets

The server URL defaults to `http://localhost:5018`. Override it if your server runs elsewhere:

```bash
dotnet user-secrets set "AGUI_SERVER_URL" "http://localhost:5018"
```

## Run

Start the server first, then run the client:

```bash
# Terminal 1 — start the server
cd ../GHModel.dotNET.AI.Workflow.AGUI.Server
dotnet run

# Terminal 2 — start the client
dotnet run
```

Type messages at the prompt. Enter `:q` or `quit` to exit.
