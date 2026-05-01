namespace CRM.Medical.Application.Abstractions.Chat;

/// <summary>
/// Redis-backed presence and connection tracking (implementation in RealTime layer).
/// Uses SET <c>presence:online-users</c> — no SCAN for enumerating online users.
/// </summary>
public interface IConnectionManager
{
    /// <param name="roles">Identity role claims; used for <c>presence:role:*</c> indexes.</param>
    Task<ConnectionAddResult> AddConnectionAsync(
        string userId,
        string connectionId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default);

    Task<ConnectionRemovalResult> RemoveConnectionAsync(string connectionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetConnectionsAsync(string userId, CancellationToken cancellationToken = default);

    Task<bool> IsOnlineAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Redis SISMEMBER against <c>presence:online-users</c> for each id (single batched network round-trip).</summary>
    Task<IReadOnlySet<string>> GetOnlineSubsetAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetAllOnlineUserIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>Reads role markers <c>presence:user:{id}:role</c> for many users in one batch.</summary>
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>?>> GetPersistedRolesForUsersAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>Comma-split roles from Redis marker <c>presence:user:{userId}:role</c>.</summary>
    Task<IReadOnlyList<string>?> GetPersistedRolesAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Members of SET <c>presence:role:{role}:users</c>.</summary>
    Task<IReadOnlyCollection<string>> GetOnlineUserIdsInRoleAsync(string role, CancellationToken cancellationToken = default);
}
