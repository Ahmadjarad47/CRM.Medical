namespace CRM.Medical.RealTime.Hubs;

/// <summary>Strongly typed SignalR client callbacks for <see cref="OnlineUsersHub"/>.</summary>
public interface IOnlineUsersClient
{
    Task UserOnline(UserOnlinePayload payload);

    Task UserOffline(UserOfflinePayload payload);

    Task OnlineUsersList(IEnumerable<UserOnlinePayload> users);
}

/// <summary>Payload for <see cref="IOnlineUsersClient.UserOnline"/>.</summary>
public sealed record UserOnlinePayload(string UserId, IReadOnlyList<string>? Roles);

/// <summary>Payload for <see cref="IOnlineUsersClient.UserOffline"/>.</summary>
public sealed record UserOfflinePayload(string UserId);
