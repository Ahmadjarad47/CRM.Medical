using CRM.Medical.Application.Abstractions.Chat;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.RealTime.Hubs;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Medical.RealTime.Presence;

/// <summary>
/// Builds <see cref="UserOnlinePayload"/> lists from Redis SET <c>presence:online-users</c>
/// and role markers / role indexes (SET per role).
/// </summary>
public sealed class OnlineUserService(IConnectionManager connections, IServiceScopeFactory scopeFactory)
    : IOnlineUserService
{
    public async Task<IReadOnlyCollection<UserOnlinePayload>> GetOnlineUsersAsync(CancellationToken cancellationToken = default)
    {
        var ids = await connections.GetAllOnlineUserIdsAsync(cancellationToken).ConfigureAwait(false);
        return await ToPayloadsAsync(ids, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<UserOnlinePayload>> GetOnlineUsersByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(role))
            return Array.Empty<UserOnlinePayload>();

        var ids = await connections.GetOnlineUserIdsInRoleAsync(role.Trim(), cancellationToken).ConfigureAwait(false);
        return await ToPayloadsAsync(ids, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyCollection<UserOnlinePayload>> ToPayloadsAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
            return Array.Empty<UserOnlinePayload>();

        var distinctList = userIds
            .Where(static u => !string.IsNullOrWhiteSpace(u))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        await using var scope = scopeFactory.CreateAsyncScope();
        var lookup = scope.ServiceProvider.GetRequiredService<IChatUserSummaryLookup>();
        var rolesMap = await connections.GetPersistedRolesForUsersAsync(distinctList, cancellationToken).ConfigureAwait(false);
        var summaries = await lookup.GetSummariesAsync(distinctList, cancellationToken).ConfigureAwait(false);

        var list = new List<UserOnlinePayload>(distinctList.Count);
        foreach (var userId in distinctList)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rolesMap.TryGetValue(userId, out var roles);
            summaries.TryGetValue(userId, out var user);
            list.Add(new UserOnlinePayload(userId, roles?.ToList(), user));
        }

        return list;
    }
}
