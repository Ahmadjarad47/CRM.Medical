using CRM.Medical.Application.Abstractions.Chat;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CRM.Medical.RealTime.Infrastructure.Redis;

/// <summary>Fallback when Redis / <see cref="IConnectionMultiplexer"/> is not registered (single-node dev).</summary>
public sealed class NullConnectionManager(ILogger<NullConnectionManager> logger) : IConnectionManager
{
    private readonly ILogger<NullConnectionManager> _logger = logger;

    public Task<ConnectionAddResult> AddConnectionAsync(
        string userId,
        string connectionId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullConnectionManager: AddConnection ignored (no Redis).");
        return Task.FromResult(new ConnectionAddResult(false));
    }

    public Task<ConnectionRemovalResult> RemoveConnectionAsync(string connectionId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ConnectionRemovalResult(null, false));

    public Task<IReadOnlyCollection<string>> GetConnectionsAsync(string userId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());

    public Task<bool> IsOnlineAsync(string userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public Task<IReadOnlyCollection<string>> GetAllOnlineUserIdsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());

    public Task<IReadOnlyList<string>?> GetPersistedRolesAsync(string userId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<string>?>(null);

    public Task<IReadOnlyCollection<string>> GetOnlineUserIdsInRoleAsync(string role, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());
}
