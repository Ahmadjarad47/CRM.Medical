namespace CRM.Medical.RealTime;

internal static class RedisChatKeys
{
    public const string ConnectionPrefix = "chat:connection:";
    public const string UserPresenceSuffix = ":presence";
    public const string UserConnectionsSuffix = ":connections";

    public static string UserConnections(string userId) => $"chat:user:{userId}:connections";

    public static string ConnectionUser(string connectionId) => $"{ConnectionPrefix}{connectionId}:user";

    public static string UserPresence(string userId) => $"chat:user:{userId}:presence";

    /// <summary>Pattern for SCAN — matches presence keys.</summary>
    public const string PresencePattern = "chat:user:*:presence";
}
