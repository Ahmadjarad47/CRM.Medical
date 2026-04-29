using System.Security.Claims;
using CRM.Medical.RealTime.Presence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.RealTime.Hubs;

/// <summary>
/// Presence hub: optional RPC (<see cref="GetOnlineUsers"/>) plus <see cref="IOnlineUsersClient.UserOnline"/> /
/// <see cref="IOnlineUsersClient.UserOffline"/> when a user transitions to first connection or last disconnect.
/// </summary>
[Authorize]
public sealed class OnlineUsersHub(
    PresenceLifecycleCoordinator presenceLifecycle,
    IOnlineUserService onlineUserService,
    ILogger<OnlineUsersHub> logger)
    : Hub<IOnlineUsersClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            var roles = Context.User!.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            await presenceLifecycle.OnHubConnectedAsync(userId, Context.ConnectionId, roles, Context.ConnectionAborted)
                .ConfigureAwait(false);
        }
        else
        {
            logger.LogWarning("OnlineUsersHub connection without NameIdentifier claim.");
        }

        await base.OnConnectedAsync().ConfigureAwait(false);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await presenceLifecycle.OnHubDisconnectedAsync(Context.ConnectionId, Context.ConnectionAborted).ConfigureAwait(false);
        await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
    }

    /// <summary>Loads the full roster from Redis SET <c>presence:online-users</c> (SMEMBERS).</summary>
    public async Task GetOnlineUsers()
    {
        // Optional: enforce admin-only access
        // await authorizationService.EnsureAdminAsync(Context.UserIdentifier);

        var users = await onlineUserService.GetOnlineUsersAsync(Context.ConnectionAborted).ConfigureAwait(false);
        await Clients.Caller.OnlineUsersList(users).ConfigureAwait(false);
    }

    /// <summary>Loads users indexed under Redis SET <c>presence:role:{role}:users</c>.</summary>
    public async Task GetOnlineUsersByRole(string role)
    {
        var users = await onlineUserService.GetOnlineUsersByRoleAsync(role, Context.ConnectionAborted).ConfigureAwait(false);
        await Clients.Caller.OnlineUsersList(users).ConfigureAwait(false);
    }
}
