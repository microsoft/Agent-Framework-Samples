using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace GHModel.AI.Workflows;

/// <summary>
/// Simple abstraction that lets the workflow ask a human ("Boss") for confirmation.
/// Replace with Copilot Studio, Teams message, email, etc. in a real system.
/// </summary>
public interface IHumanApprovalService
{
    Task<string> AskAsync(string prompt, CancellationToken cancellationToken = default);
}

/// <summary>
/// Console-based implementation for local testing.
/// </summary>
public sealed class ConsoleHumanApprovalService : IHumanApprovalService
{
    public Task<string> AskAsync(string prompt, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(prompt);
        var answer = Console.ReadLine() ?? string.Empty;
        return Task.FromResult(answer.Trim());
    }
}

/// <summary>
/// .NET translation of the HRDemo declarative workflow. The workflow is triggered on conversation start.
/// 1. Capture the user's latest message.
/// 2. Send the conversation to the HiringManager agent.
/// 3. Ask the boss for confirmation. If the answer is not "Yes", route back to the HiringManager agent.
/// 4. Once confirmed, send the latest message to the ApplyAgent.
/// 5. End the conversation.
/// </summary>
public sealed class HrDemoWorkflow
{
    private readonly AIAgent _hiringManagerAgent;
    private readonly AIAgent _applyAgent;
    private readonly IHumanApprovalService _approvalService;

    public HrDemoWorkflow(AIAgent hiringManagerAgent, AIAgent applyAgent, IHumanApprovalService approvalService)
    {
        _hiringManagerAgent = hiringManagerAgent ?? throw new ArgumentNullException(nameof(hiringManagerAgent));
        _applyAgent = applyAgent ?? throw new ArgumentNullException(nameof(applyAgent));
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
    }

    /// <summary>
    /// Runs the workflow logic starting from the user's conversation payload.
    /// </summary>
    /// <param name="conversation">Existing conversation messages (System.LastMessage in declarative workflow).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated transcript with all agent outputs.</returns>
    public async Task<IReadOnlyList<ChatMessage>> RunAsync(
        IReadOnlyList<ChatMessage> conversation,
        CancellationToken cancellationToken = default)
    {
        if (conversation == null || conversation.Count == 0)
        {
            throw new ArgumentException("Conversation must contain at least one message.", nameof(conversation));
        }

        HrDemoWorkflowState state = new()
        {
            LatestMessage = conversation.LastOrDefault(m => m.Role == ChatRole.User)?.Content ?? string.Empty,
        };

        // Step 1 + 2: invoke HiringManager agent immediately when the conversation starts.
        var transcript = new List<ChatMessage>(conversation);
        state.LatestMessage = await InvokeAgentAsync(
            _hiringManagerAgent,
            transcript,
            nameof(_hiringManagerAgent),
            cancellationToken);

        transcript.Add(new ChatMessage(ChatRole.Assistant, state.LatestMessage));

        // Step 3 + loop: ask for approval; reroute to the hiring manager until the boss says "Yes".
        state.Input = await _approvalService.AskAsync("Boss, can you confirm this ?", cancellationToken);
        while (!state.IsApproved)
        {
            state.LatestMessage = await InvokeAgentAsync(
                _hiringManagerAgent,
                new[] { new ChatMessage(ChatRole.User, state.LatestMessage ?? string.Empty) },
                nameof(_hiringManagerAgent),
                cancellationToken);

            transcript.Add(new ChatMessage(ChatRole.Assistant, state.LatestMessage));
            state.Input = await _approvalService.AskAsync("Boss, can you confirm this ?", cancellationToken);
        }

        // Step 4: invoke ApplyAgent when approval is granted.
        string applyResult = await InvokeAgentAsync(
            _applyAgent,
            new[] { new ChatMessage(ChatRole.User, state.LatestMessage ?? string.Empty) },
            nameof(_applyAgent),
            cancellationToken);

        transcript.Add(new ChatMessage(ChatRole.Assistant, applyResult));
        return transcript;
    }

    private static async Task<string> InvokeAgentAsync(
        AIAgent agent,
        IReadOnlyList<ChatMessage> messages,
        string agentDisplayName,
        CancellationToken cancellationToken)
    {
        var agentResponse = await agent.RunAsync(messages, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(agentResponse.Text))
        {
            throw new InvalidOperationException($"{agentDisplayName} returned an empty response.");
        }

        return agentResponse.Text;
    }

    private sealed class HrDemoWorkflowState
    {
        public string? LatestMessage { get; set; }

        public string? Input { get; set; }

        public bool IsApproved => string.Equals(Input, "Yes", StringComparison.OrdinalIgnoreCase);
    }
}
