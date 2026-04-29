using CRM.Medical.Application.Features.Chat;
using CRM.Medical.Application.Features.Chat.Models;
using CRM.Medical.Application.Features.Chat.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.RealTime;

public sealed class ChatRealtimeNotifier(IHubContext<ChatHub> hubContext, ILogger<ChatRealtimeNotifier> logger)
    : IChatRealtimeNotifier
{
    private readonly IHubContext<ChatHub> _hubContext = hubContext;
    private readonly ILogger<ChatRealtimeNotifier> _logger = logger;

    public async Task BroadcastNewMessageAsync(
        Guid conversationId,
        ChatMessageRealtimePayload payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(ChatGroups.Conversation(conversationId))
                .SendAsync(ChatHubClientMethods.ReceiveMessage, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR broadcast failed for conversation {ConversationId}", conversationId);
        }
    }

    public async Task BroadcastTypingAsync(
        Guid conversationId,
        ChatTypingPayload payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(ChatGroups.Conversation(conversationId))
                .SendAsync(ChatHubClientMethods.TypingIndicator, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR typing broadcast failed for conversation {ConversationId}", conversationId);
        }
    }

    public async Task BroadcastReadReceiptAsync(
        Guid conversationId,
        ChatReadReceiptPayload payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(ChatGroups.Conversation(conversationId))
                .SendAsync(ChatHubClientMethods.ReadReceipt, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR read receipt failed for conversation {ConversationId}", conversationId);
        }
    }

    public async Task NotifyUserAsync(
        string userId,
        string methodName,
        object payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(ChatGroups.User(userId))
                .SendAsync(methodName, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR user notify failed for {UserId}", userId);
        }
    }
}
