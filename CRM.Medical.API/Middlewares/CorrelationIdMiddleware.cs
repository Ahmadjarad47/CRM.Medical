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

        string correlationId;

        if (context.Request.Headers.TryGetValue(_options.HeaderName, out var headerValue)
            && !string.IsNullOrWhiteSpace(headerValue))
        {
            correlationId = headerValue!;
        }
        else if (_options.UseTraceIdentifierWhenMissing)
        {
            correlationId = context.TraceIdentifier;
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();
        }

        if (_options.IncludeInResponse)
            context.Response.Headers[_options.HeaderName] = correlationId;

        await next(context);
    }
}
