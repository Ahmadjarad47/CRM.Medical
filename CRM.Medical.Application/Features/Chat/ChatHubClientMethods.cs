namespace CRM.Medical.Application.Features.Chat;

/// <summary>SignalR client callback names used by <see cref="Services.IChatRealtimeNotifier"/>.</summary>
public static class ChatHubClientMethods
{
    public const string ReceiveMessage = "ReceiveMessage";

    public const string TypingIndicator = "Typing";

    public const string StopTypingIndicator = "StopTyping";

    public const string ReadReceipt = "ReadReceipt";

    /// <summary>Pushed to user:{userId} group for badges/list refresh.</summary>
    public const string ConversationUpdated = "ConversationUpdated";
}
