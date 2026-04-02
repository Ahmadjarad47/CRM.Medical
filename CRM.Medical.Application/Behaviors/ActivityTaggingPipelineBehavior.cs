using System.Diagnostics;
using MediatR;

namespace CRM.Medical.Application.Behaviors;

public sealed class ActivityTaggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Activity.Current?.SetTag("mediatR.request", typeof(TRequest).Name);
        return await next(cancellationToken);
    }
}
