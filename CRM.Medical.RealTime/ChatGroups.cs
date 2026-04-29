namespace CRM.Medical.RealTime;

public static class ChatGroups
{
    public static string Conversation(Guid conversationId) => $"conversation:{conversationId}";

    public static string User(string userId) => $"user:{userId}";
}
