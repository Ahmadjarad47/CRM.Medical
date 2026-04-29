namespace CRM.Medical.Application.Abstractions.Chat;

/// <summary>Result of registering a SignalR connection in Redis-backed presence.</summary>
public sealed record ConnectionAddResult(bool BecameOnline);

/// <summary>Result of removing a SignalR connection (includes offline detection).</summary>
public sealed record ConnectionRemovalResult(string? UserId, bool IsNowOffline);

/// <summary>
/// Broadcasts global presence transitions (first connection / last disconnect) to the online-users SignalR hub.
/// </summary>
public interface IPresenceEventNotifier
{
    Task NotifyUserBecameOnlineAsync(string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default);

    Task NotifyUserBecameOfflineAsync(string userId, CancellationToken cancellationToken = default);
}
