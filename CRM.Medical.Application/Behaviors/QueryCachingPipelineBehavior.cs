using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Application.Behaviors;

public sealed class QueryCachingPipelineBehavior<TRequest, TResponse>(
    ICacheService cache,
    ICurrentUserAccessor currentUser,
    ILogger<QueryCachingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!IsQuery())
            return await next(cancellationToken);

        var cacheKey = BuildCacheKey(request);
        var cached = await cache.GetAsync<CachedQueryResponse<TResponse>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            logger.LogDebug("Query cache hit for {RequestName}", typeof(TRequest).Name);
            return cached.Value;
        }

        var response = await next(cancellationToken);
        await cache.SetAsync(
            cacheKey,
            new CachedQueryResponse<TResponse>(response),
            CacheKeys.QueryResponseExpiry,
            cancellationToken);

        logger.LogDebug("Query cache set for {RequestName}", typeof(TRequest).Name);
        return response;
    }

    private static bool IsQuery() =>
        typeof(TRequest).Name.EndsWith("Query", StringComparison.Ordinal);

    private string BuildCacheKey(TRequest request)
    {
        var payload = new QueryCacheKeyPayload<TRequest>(
            typeof(TRequest).FullName ?? typeof(TRequest).Name,
            request,
            currentUser.UserId,
            currentUser.TenantId,
            currentUser.Roles
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToArray());

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
        return CacheKeys.QueryResponse(typeof(TRequest).Name, hash);
    }

    private sealed record QueryCacheKeyPayload<TPayload>(
        string RequestType,
        TPayload Request,
        string? UserId,
        string? TenantId,
        IReadOnlyList<string> Roles);

    private sealed record CachedQueryResponse<TValue>(TValue Value);
}
