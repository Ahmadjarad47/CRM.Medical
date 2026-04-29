using CRM.Medical.RealTime.Hubs;

namespace CRM.Medical.RealTime.Presence;

/// <summary>Resolves current online users from Redis SET indexes (no SCAN).</summary>
public interface IOnlineUserService
{
    Task<IReadOnlyCollection<UserOnlinePayload>> GetOnlineUsersAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserOnlinePayload>> GetOnlineUsersByRoleAsync(string role, CancellationToken cancellationToken = default);
}
