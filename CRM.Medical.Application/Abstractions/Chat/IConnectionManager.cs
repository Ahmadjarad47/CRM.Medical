namespace CRM.Medical.Application.Abstractions.Chat;

/// <summary>
/// Redis-backed presence and connection tracking for real-time chat (implementation in RealTime layer).
/// </summary>
public interface IConnectionManager
{
    Task AddConnectionAsync(string userId, string connectionId, CancellationToken cancellationToken = default);

    Task RemoveConnectionAsync(string connectionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetConnectionsAsync(string userId, CancellationToken cancellationToken = default);

    Task<bool> IsOnlineAsync(string userId, CancellationToken cancellationToken = default);

    Task SetUserOnlineAsync(string userId, CancellationToken cancellationToken = default);

    Task SetUserOfflineAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Returns user ids that have a presence key (online for this deployment / Redis).</summary>
    Task<IReadOnlyCollection<string>> GetAllOnlineUserIdsAsync(CancellationToken cancellationToken = default);
}
