namespace CRM.Medical.RealTime.Infrastructure.Redis;

/// <summary>Redis keys for connection-scoped presence (SET-based; no SCAN for listing online users).</summary>
internal static class PresenceRedisKeys
{
    /// <summary>SET of user ids currently considered online (≥1 active SignalR connection).</summary>
    public const string OnlineUsersSet = "presence:online-users";

    public static string RoleUsers(string role) => $"presence:role:{role}:users";

    public static string UserConnections(string userId) => $"presence:user:{userId}:connections";

    public static string ConnectionUser(string connectionId) => $"presence:connection:{connectionId}:user";

    /// <summary>STRING: comma-separated roles snapshot for disconnect cleanup.</summary>
    public static string UserRolesMarker(string userId) => $"presence:user:{userId}:role";
}
