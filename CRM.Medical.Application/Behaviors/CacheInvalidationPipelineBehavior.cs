using CRM.Medical.Application.Common.Caching;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Application.Behaviors;

public sealed class CacheInvalidationPipelineBehavior<TRequest, TResponse>(
    ICacheService cache,
    ILogger<CacheInvalidationPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly HashSet<string> NonMutatingCommands = new(StringComparer.Ordinal)
    {
        "LoginCommand",
        "RefreshTokenCommand"
    };

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        if (ShouldInvalidate())
        {
            await cache.RemoveByPrefixAsync(CacheKeys.QueryResponsePrefix, cancellationToken);
            logger.LogDebug("Query cache invalidated after {RequestName}", typeof(TRequest).Name);
        }

        return response;
    }

    private static bool ShouldInvalidate()
    {
        var requestName = typeof(TRequest).Name;
        return requestName.EndsWith("Command", StringComparison.Ordinal)
            && !NonMutatingCommands.Contains(requestName);
    }
}
