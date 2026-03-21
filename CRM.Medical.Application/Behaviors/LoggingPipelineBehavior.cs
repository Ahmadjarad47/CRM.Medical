using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Application.Behaviors;

public sealed class LoggingPipelineBehavior<TRequest, TResponse>(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;
        logger.LogInformation("MediatR begin {RequestName}", name);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();
            logger.LogInformation(
                "MediatR end {RequestName} in {ElapsedMs}ms",
                name,
                stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(
                ex,
                "MediatR failed {RequestName} after {ElapsedMs}ms",
                name,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
