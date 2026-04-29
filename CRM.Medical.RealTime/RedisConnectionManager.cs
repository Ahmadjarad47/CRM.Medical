using CRM.Medical.Application.Abstractions.Chat;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CRM.Medical.RealTime;

/// <summary>
/// Tracks SignalR connections per user using Redis sets + reverse lookup per connection id.
/// </summary>
public sealed class RedisConnectionManager(IConnectionMultiplexer mux, ILogger<RedisConnectionManager> logger)
    : IConnectionManager
{
    private readonly IDatabase _db = mux.GetDatabase();
    private readonly IConnectionMultiplexer _mux = mux;
    private readonly ILogger<RedisConnectionManager> _logger = logger;

    public async Task AddConnectionAsync(string userId, string connectionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(connectionId))
            return;

        var userKey = RedisChatKeys.UserConnections(userId);
        var connMap = RedisChatKeys.ConnectionUser(connectionId);

        await _db.SetAddAsync(userKey, connectionId).ConfigureAwait(false);
        await _db.StringSetAsync(connMap, userId, TimeSpan.FromHours(24), when: When.Always).ConfigureAwait(false);
        await _db.StringSetAsync(RedisChatKeys.UserPresence(userId), "1").ConfigureAwait(false);
    }

    public async Task RemoveConnectionAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return;

        var connMap = RedisChatKeys.ConnectionUser(connectionId);
        var userId = await _db.StringGetAsync(connMap).ConfigureAwait(false);
        await _db.KeyDeleteAsync(connMap).ConfigureAwait(false);

        if (userId.IsNullOrEmpty)
        {
            _logger.LogDebug("RemoveConnection: missing user mapping for connection {ConnectionId}", connectionId);
            return;
        }

        var uid = userId.ToString();
        var userKey = RedisChatKeys.UserConnections(uid);
        await _db.SetRemoveAsync(userKey, connectionId).ConfigureAwait(false);

        var remaining = await _db.SetLengthAsync(userKey).ConfigureAwait(false);
        if (remaining == 0)
        {
            await _db.KeyDeleteAsync(userKey).ConfigureAwait(false);
            await _db.KeyDeleteAsync(RedisChatKeys.UserPresence(uid)).ConfigureAwait(false);
        }
    }

    public async Task<IReadOnlyCollection<string>> GetConnectionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var members = await _db.SetMembersAsync(RedisChatKeys.UserConnections(userId)).ConfigureAwait(false);
        return members.Select(m => m.ToString()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
    }

    public async Task<bool> IsOnlineAsync(string userId, CancellationToken cancellationToken = default)
    {
        var exists = await _db.KeyExistsAsync(RedisChatKeys.UserPresence(userId)).ConfigureAwait(false);
        if (exists)
            return true;

        var count = await _db.SetLengthAsync(RedisChatKeys.UserConnections(userId)).ConfigureAwait(false);
        return count > 0;
    }

    public Task SetUserOnlineAsync(string userId, CancellationToken cancellationToken = default) =>
        _db.StringSetAsync(RedisChatKeys.UserPresence(userId), "1");

    public async Task SetUserOfflineAsync(string userId, CancellationToken cancellationToken = default)
    {
        await _db.KeyDeleteAsync(RedisChatKeys.UserPresence(userId)).ConfigureAwait(false);
        await _db.KeyDeleteAsync(RedisChatKeys.UserConnections(userId)).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<string>> GetAllOnlineUserIdsAsync(CancellationToken cancellationToken = default)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        try
        {
            foreach (var endpoint in _mux.GetEndPoints())
            {
                var server = _mux.GetServer(endpoint);
                if (!server.IsConnected)
                    continue;

                foreach (var key in server.Keys(pattern: RedisChatKeys.PresencePattern, pageSize: 512))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (ExtractUserIdFromPresenceKey(key.ToString()) is { } id)
                        set.Add(id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to enumerate online presence keys.");
        }

        return set.ToList();
    }

    internal static string? ExtractUserIdFromPresenceKey(string redisKey)
    {
        const string prefix = "chat:user:";
        const string suffix = ":presence";
        if (!redisKey.StartsWith(prefix, StringComparison.Ordinal) || !redisKey.EndsWith(suffix, StringComparison.Ordinal))
            return null;

        return redisKey[prefix.Length..^suffix.Length];
    }
}
