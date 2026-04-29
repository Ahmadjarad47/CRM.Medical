using CRM.Medical.Application.Abstractions.Chat;
using CRM.Medical.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.RealTime.Presence;

public sealed class OnlineUsersPresenceNotifier(
    IHubContext<OnlineUsersHub, IOnlineUsersClient> hubContext,
    ILogger<OnlineUsersPresenceNotifier> logger)
    : IPresenceEventNotifier
{
    private readonly IHubContext<OnlineUsersHub, IOnlineUsersClient> _hubContext = hubContext;
    private readonly ILogger<OnlineUsersPresenceNotifier> _logger = logger;

    public async Task NotifyUserBecameOnlineAsync(
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All
                .UserOnline(new UserOnlinePayload(userId, roles.ToList()));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to broadcast UserOnline for {UserId}", userId);
        }
    }

    public async Task NotifyUserBecameOfflineAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All
                .UserOffline(new UserOfflinePayload(userId));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to broadcast UserOffline for {UserId}", userId);
        }
    }
}
