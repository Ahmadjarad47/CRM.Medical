using CRM.Medical.Application.Abstractions.Chat;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CRM.Medical.RealTime.Infrastructure.Redis;

/// <summary>
/// Tracks SignalR connections per user using Redis SETs (including global online users); removal reports offline transitions.
/// </summary>
public sealed class RedisConnectionManager(IConnectionMultiplexer mux, ILogger<RedisConnectionManager> logger)
    : IConnectionManager
{
    private readonly IDatabase _db = mux.GetDatabase();
    private readonly ILogger<RedisConnectionManager> _logger = logger;

    public async Task<ConnectionAddResult> AddConnectionAsync(
        string userId,
        string connectionId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(connectionId))
            return new ConnectionAddResult(false);

        var incoming = roles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var userConnectionsKey = PresenceRedisKeys.UserConnections(userId);
        var connMapKey = PresenceRedisKeys.ConnectionUser(connectionId);

        await _db.SetAddAsync(userConnectionsKey, connectionId).ConfigureAwait(false);
        await _db.StringSetAsync(connMapKey, userId, TimeSpan.FromHours(24), when: When.Always).ConfigureAwait(false);

        var mergedRoles = await MergeAndPersistRolesMarkerAsync(userId, incoming).ConfigureAwait(false);

        var connectionCount = await _db.SetLengthAsync(userConnectionsKey).ConfigureAwait(false);
        var becameOnline = connectionCount == 1;

        if (becameOnline)
            await _db.SetAddAsync(PresenceRedisKeys.OnlineUsersSet, userId).ConfigureAwait(false);

        foreach (var role in mergedRoles)
            await _db.SetAddAsync(PresenceRedisKeys.RoleUsers(role), userId).ConfigureAwait(false);

        return new ConnectionAddResult(becameOnline);
    }

    public async Task<ConnectionRemovalResult> RemoveConnectionAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return new ConnectionRemovalResult(null, false);

        var connMapKey = PresenceRedisKeys.ConnectionUser(connectionId);
        var userIdRedis = await _db.StringGetAsync(connMapKey).ConfigureAwait(false);
        await _db.KeyDeleteAsync(connMapKey).ConfigureAwait(false);

        if (userIdRedis.IsNullOrEmpty)
        {
            _logger.LogDebug("RemoveConnection: missing user mapping for connection {ConnectionId}", connectionId);
            return new ConnectionRemovalResult(null, false);
        }

        var uid = userIdRedis.ToString();
        var userConnectionsKey = PresenceRedisKeys.UserConnections(uid);
        await _db.SetRemoveAsync(userConnectionsKey, connectionId).ConfigureAwait(false);

        var remaining = await _db.SetLengthAsync(userConnectionsKey).ConfigureAwait(false);
        if (remaining > 0)
            return new ConnectionRemovalResult(uid, false);

        await _db.KeyDeleteAsync(userConnectionsKey).ConfigureAwait(false);

        await _db.SetRemoveAsync(PresenceRedisKeys.OnlineUsersSet, uid).ConfigureAwait(false);

        var rolesCsv = await _db.StringGetAsync(PresenceRedisKeys.UserRolesMarker(uid)).ConfigureAwait(false);
        await _db.KeyDeleteAsync(PresenceRedisKeys.UserRolesMarker(uid)).ConfigureAwait(false);

        if (!rolesCsv.IsNullOrEmpty)
        {
            foreach (var role in rolesCsv.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                await _db.SetRemoveAsync(PresenceRedisKeys.RoleUsers(role), uid).ConfigureAwait(false);
        }

        return new ConnectionRemovalResult(uid, true);
    }

    public async Task<IReadOnlyCollection<string>> GetConnectionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var members = await _db.SetMembersAsync(PresenceRedisKeys.UserConnections(userId)).ConfigureAwait(false);
        return members.Select(m => m.ToString()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
    }

    public Task<bool> IsOnlineAsync(string userId, CancellationToken cancellationToken = default) =>
        _db.SetContainsAsync(PresenceRedisKeys.OnlineUsersSet, userId);

    public async Task<IReadOnlySet<string>> GetOnlineSubsetAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var distinct = userIds
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (distinct.Length == 0)
            return new HashSet<string>(StringComparer.Ordinal);

        var batch = _db.CreateBatch();
        var tasks = distinct.Select(id => batch.SetContainsAsync(PresenceRedisKeys.OnlineUsersSet, id)).ToArray();
        batch.Execute();
        var flags = await Task.WhenAll(tasks).ConfigureAwait(false);

        var set = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < distinct.Length; i++)
        {
            if (flags[i])
                set.Add(distinct[i]);
        }

        return set;
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>?>> GetPersistedRolesForUsersAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var distinct = userIds
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (distinct.Length == 0)
            return new Dictionary<string, IReadOnlyList<string>?>(StringComparer.Ordinal);

        var batch = _db.CreateBatch();
        var tasks = distinct.Select(id => batch.StringGetAsync(PresenceRedisKeys.UserRolesMarker(id))).ToArray();
        batch.Execute();
        var values = await Task.WhenAll(tasks).ConfigureAwait(false);

        var dict = new Dictionary<string, IReadOnlyList<string>?>(StringComparer.Ordinal);
        for (var i = 0; i < distinct.Length; i++)
        {
            var redisVal = values[i];
            if (redisVal.IsNullOrEmpty)
            {
                dict[distinct[i]] = null;
                continue;
            }

            var parts = redisVal.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            dict[distinct[i]] = parts.Length == 0 ? null : parts;
        }

        return dict;
    }

    public async Task<IReadOnlyCollection<string>> GetAllOnlineUserIdsAsync(CancellationToken cancellationToken = default)
    {
        var members = await _db.SetMembersAsync(PresenceRedisKeys.OnlineUsersSet).ConfigureAwait(false);
        return members.Select(m => m.ToString()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
    }

    public async Task<IReadOnlyList<string>?> GetPersistedRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        var rolesCsv = await _db.StringGetAsync(PresenceRedisKeys.UserRolesMarker(userId)).ConfigureAwait(false);
        if (rolesCsv.IsNullOrEmpty)
            return null;

        var parts = rolesCsv.ToString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length == 0 ? null : parts;
    }

    public async Task<IReadOnlyCollection<string>> GetOnlineUserIdsInRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(role))
            return Array.Empty<string>();

        var members = await _db.SetMembersAsync(PresenceRedisKeys.RoleUsers(role.Trim())).ConfigureAwait(false);
        return members.Select(m => m.ToString()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
    }

    private async Task<IReadOnlyList<string>> MergeAndPersistRolesMarkerAsync(string userId, string[] incomingRoles)
    {
        var markerKey = PresenceRedisKeys.UserRolesMarker(userId);
        var existing = await _db.StringGetAsync(markerKey).ConfigureAwait(false);

        IEnumerable<string>? existingParts = existing.IsNullOrEmpty
            ? null
            : existing.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var merged = MergeRoleLists(existingParts, incomingRoles);
        if (merged.Count > 0)
            await _db.StringSetAsync(markerKey, string.Join(',', merged)).ConfigureAwait(false);
        else if (!existing.IsNullOrEmpty)
            await _db.KeyDeleteAsync(markerKey).ConfigureAwait(false);

        return merged;
    }

    private static List<string> MergeRoleLists(IEnumerable<string>? existingParts, string[] incoming)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (existingParts is not null)
        {
            foreach (var x in existingParts)
            {
                if (!string.IsNullOrWhiteSpace(x))
                    set.Add(x.Trim());
            }
        }

        foreach (var x in incoming)
        {
            if (!string.IsNullOrWhiteSpace(x))
                set.Add(x.Trim());
        }

        return set.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList();
    }
}
