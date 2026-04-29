using CRM.Medical.Application.Features.Chat.Models;

namespace CRM.Medical.Application.Features.Chat.Services;

/// <summary>
/// Broadcasts chat events over SignalR (implemented in RealTime layer).
/// </summary>
public interface IChatRealtimeNotifier
{
    Task BroadcastNewMessageAsync(
        Guid conversationId,
        ChatMessageRealtimePayload payload,
        CancellationToken cancellationToken = default);

    Task BroadcastTypingAsync(
        Guid conversationId,
        ChatTypingPayload payload,
        CancellationToken cancellationToken = default);

    Task BroadcastReadReceiptAsync(
        Guid conversationId,
        ChatReadReceiptPayload payload,
        CancellationToken cancellationToken = default);

    Task NotifyUserAsync(
        string userId,
        string methodName,
        object payload,
        CancellationToken cancellationToken = default);
}
