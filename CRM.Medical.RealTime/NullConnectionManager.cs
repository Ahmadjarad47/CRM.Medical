using CRM.Medical.Application.Abstractions.Chat;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.RealTime;

/// <summary>Fallback when Redis / <see cref="StackExchange.Redis.IConnectionMultiplexer"/> is not registered (single-node dev).</summary>
public sealed class NullConnectionManager(ILogger<NullConnectionManager> logger) : IConnectionManager
{
    private readonly ILogger<NullConnectionManager> _logger = logger;

    public Task AddConnectionAsync(string userId, string connectionId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullConnectionManager: AddConnection ignored (no Redis).");
        return Task.CompletedTask;
    }

    public Task RemoveConnectionAsync(string connectionId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<IReadOnlyCollection<string>> GetConnectionsAsync(string userId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());

    public Task<bool> IsOnlineAsync(string userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public Task SetUserOnlineAsync(string userId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task SetUserOfflineAsync(string userId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<IReadOnlyCollection<string>> GetAllOnlineUserIdsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());
}
