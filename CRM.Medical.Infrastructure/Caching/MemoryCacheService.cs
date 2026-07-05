using System.Collections.Concurrent;
using CRM.Medical.Application.Common.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Caching;

public sealed class MemoryCacheService(
    IMemoryCache cache,
    ILogger<MemoryCacheService> logger)
    : ICacheService
{
    private readonly ConcurrentDictionary<string, byte> keys = new(StringComparer.Ordinal);

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        try
        {
            return Task.FromResult(cache.TryGetValue<T>(key, out var value) ? value : null);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Memory cache GET failed for key '{Key}'. Returning null.", key);
            return Task.FromResult<T?>(null);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        where T : class
    {
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };

            options.RegisterPostEvictionCallback(static (evictedKey, _, _, state) =>
            {
                if (evictedKey is string key && state is ConcurrentDictionary<string, byte> trackedKeys)
                    trackedKeys.TryRemove(key, out _);
            }, keys);

            keys.TryAdd(key, 0);
            cache.Set(key, value, options);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Memory cache SET failed for key '{Key}'. Continuing without cache.", key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try
        {
            cache.Remove(key);
            keys.TryRemove(key, out _);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Memory cache REMOVE failed for key '{Key}'.", key);
        }

        return Task.CompletedTask;
    }

    public async Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken ct = default)
    {
        foreach (var key in keys)
            await RemoveAsync(key, ct);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var matches = keys.Keys
            .Where(key => key.StartsWith(prefix, StringComparison.Ordinal))
            .ToArray();

        foreach (var key in matches)
            await RemoveAsync(key, ct);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        foreach (var key in keys.Keys.ToArray())
            await RemoveAsync(key, ct);
    }
}
