using CRM.Medical.Application.Abstractions.Chat;
using CRM.Medical.Application.Features.Chat.DTOs;

namespace CRM.Medical.Application.Features.Chat.Services;

public sealed class ChatUserSummaryLookup(IChatUserProfileReader profiles, IConnectionManager connections)
    : IChatUserSummaryLookup
{
    private readonly IChatUserProfileReader _profiles = profiles;
    private readonly IConnectionManager _connections = connections;

    public async Task<IReadOnlyDictionary<string, ChatUserSummaryDto>> GetSummariesAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.Where(static id => !string.IsNullOrWhiteSpace(id)).Distinct(StringComparer.Ordinal).ToArray();
        if (ids.Length == 0)
            return new Dictionary<string, ChatUserSummaryDto>(StringComparer.Ordinal);

        var snapshots = await _profiles.GetProfilesAsync(ids, cancellationToken).ConfigureAwait(false);
        var online = await _connections.GetOnlineSubsetAsync(ids, cancellationToken).ConfigureAwait(false);

        var dict = new Dictionary<string, ChatUserSummaryDto>(StringComparer.Ordinal);
        foreach (var id in ids)
        {
            snapshots.TryGetValue(id, out var p);
            var fullName = ResolveDisplayName(p);
            dict[id] = new ChatUserSummaryDto(
                id,
                fullName,
                p?.Email,
                p?.PhoneNumber,
                p?.Role,
                online.Contains(id));
        }

        return dict;
    }

    private static string ResolveDisplayName(ChatUserProfileSnapshot? p)
    {
        if (!string.IsNullOrWhiteSpace(p?.FullName))
            return p.FullName.Trim();

        if (!string.IsNullOrWhiteSpace(p?.Email))
            return p.Email.Trim();

        return "Unknown user";
    }
}
