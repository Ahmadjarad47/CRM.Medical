using CRM.Medical.Application.Abstractions.Chat;

namespace CRM.Medical.RealTime.Presence;

/// <summary>
/// Shared connect/disconnect handling for hubs that participate in Redis presence (avoids duplicate broadcast logic).
/// </summary>
public sealed class PresenceLifecycleCoordinator(IConnectionManager connections, IPresenceEventNotifier presence)
{
    public async Task OnHubConnectedAsync(
        string userId,
        string connectionId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        var result = await connections.AddConnectionAsync(userId, connectionId, roles, cancellationToken).ConfigureAwait(false);
        if (result.BecameOnline)
            await presence.NotifyUserBecameOnlineAsync(userId, roles, cancellationToken).ConfigureAwait(false);
    }

    public async Task OnHubDisconnectedAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        var result = await connections.RemoveConnectionAsync(connectionId, cancellationToken).ConfigureAwait(false);
        if (result is { IsNowOffline: true, UserId: { } uid })
            await presence.NotifyUserBecameOfflineAsync(uid, cancellationToken).ConfigureAwait(false);
    }
}
