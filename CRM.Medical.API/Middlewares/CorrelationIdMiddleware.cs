using Microsoft.Extensions.Options;

namespace CRM.Medical.API.Middlewares;

public sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    IOptions<CorrelationIdMiddlewareOptions> options)
{
    private readonly CorrelationIdMiddlewareOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.Enabled)
        {
            await next(context);
            return;
        }

        var headerName = _options.HeaderName;
        var incomingCorrelationId = context.Request.Headers[headerName].FirstOrDefault();
        var correlationId = string.IsNullOrWhiteSpace(incomingCorrelationId)
            ? (_options.UseTraceIdentifierWhenMissing ? context.TraceIdentifier : Guid.NewGuid().ToString("N"))
            : incomingCorrelationId.Trim();

        context.Items[headerName] = correlationId;
        context.TraceIdentifier = correlationId;

        if (_options.IncludeInResponse)
            context.Response.Headers[headerName] = correlationId;

        await next(context);
    }
}
