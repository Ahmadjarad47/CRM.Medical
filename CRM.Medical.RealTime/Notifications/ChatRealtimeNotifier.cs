using CRM.Medical.Application.Features.Chat.Models;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.RealTime.Groups;
using CRM.Medical.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.RealTime.Notifications;

public sealed class ChatRealtimeNotifier(IHubContext<ChatHub, IChatClient> hubContext, ILogger<ChatRealtimeNotifier> logger)
    : IChatRealtimeNotifier
{
    private readonly IHubContext<ChatHub, IChatClient> _hubContext = hubContext;
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
                .ReceiveMessage(payload);
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
                .Typing(payload);
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
                .ReadReceipt(payload);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR read receipt failed for conversation {ConversationId}", conversationId);
        }
    }

    public async Task NotifyConversationUpdatedAsync(
        string userId,
        ConversationUpdatedPayload payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(ChatGroups.User(userId))
                .ConversationUpdated(payload);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR ConversationUpdated failed for {UserId}", userId);
        }
    }
}
