using CRM.Medical.Application.Abstractions.Chat;
using CRM.Medical.RealTime.Hubs;

namespace CRM.Medical.RealTime.Presence;

/// <summary>
/// Builds <see cref="UserOnlinePayload"/> lists from Redis SET <c>presence:online-users</c>
/// and role markers / role indexes (SET per role).
/// </summary>
public sealed class OnlineUserService(IConnectionManager connections) : IOnlineUserService
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

        var list = new List<UserOnlinePayload>(userIds.Count);
        foreach (var userId in userIds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var roles = await connections.GetPersistedRolesAsync(userId, cancellationToken).ConfigureAwait(false);
            list.Add(new UserOnlinePayload(userId, roles));
        }

        return list;
    }
}
