using System.Diagnostics;
using MediatR;

namespace CRM.Medical.Application.Behaviors;

/// <summary>
/// Enriches the current <see cref="Activity"/> (when present) for tracing dashboards.
/// Runs inside <see cref="LoggingPipelineBehavior{TRequest,TResponse}"/> so tags cover handler execution.
/// </summary>
public sealed class ActivityTaggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var activity = Activity.Current;
        if (activity is not null)
        {
            activity.SetTag("mediatr.request_type", typeof(TRequest).FullName);
            activity.SetTag("mediatr.response_type", typeof(TResponse).FullName);
        }

        return await next(cancellationToken);
    }
}
