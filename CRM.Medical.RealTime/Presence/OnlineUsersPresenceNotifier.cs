using CRM.Medical.Application.Abstractions.Chat;
using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.RealTime.Presence;

public sealed class OnlineUsersPresenceNotifier(
    IHubContext<OnlineUsersHub, IOnlineUsersClient> hubContext,
    ILogger<OnlineUsersPresenceNotifier> logger,
    IServiceScopeFactory scopeFactory)
    : IPresenceEventNotifier
{
    public async Task NotifyUserBecameOnlineAsync(
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        ChatUserSummaryDto? summary = null;
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var lookup = scope.ServiceProvider.GetRequiredService<IChatUserSummaryLookup>();
            var map = await lookup.GetSummariesAsync([userId], cancellationToken).ConfigureAwait(false);
            map.TryGetValue(userId, out summary);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load user summary for UserOnline {UserId}", userId);
        }

        try
        {
            await hubContext.Clients.All
                .UserOnline(new UserOnlinePayload(userId, roles.ToList(), summary));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to broadcast UserOnline for {UserId}", userId);
        }
    }

    public async Task NotifyUserBecameOfflineAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await hubContext.Clients.All
                .UserOffline(new UserOfflinePayload(userId));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to broadcast UserOffline for {UserId}", userId);
        }
    }
}
