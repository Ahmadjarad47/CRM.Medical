using CRM.Medical.Application.Features.Chat.Models;

namespace CRM.Medical.RealTime.Hubs;

/// <summary>Strongly typed SignalR client callbacks for <see cref="ChatHub"/>.</summary>
public interface IChatClient
{
    Task ReceiveMessage(ChatMessageRealtimePayload payload);

    Task Typing(ChatTypingPayload payload);

    Task StopTyping(ChatTypingPayload payload);

    Task ReadReceipt(ChatReadReceiptPayload payload);

    Task ConversationUpdated(ConversationUpdatedPayload payload);
}
