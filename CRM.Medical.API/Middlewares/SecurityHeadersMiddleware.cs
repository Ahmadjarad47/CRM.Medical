using Microsoft.Extensions.Options;

namespace CRM.Medical.API.Middlewares;

public sealed class SecurityHeadersMiddleware(
    RequestDelegate next,
    IHostEnvironment environment,
    IOptions<SecurityHeadersMiddlewareOptions> options)
{
    private readonly SecurityHeadersMiddlewareOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.Enabled)
        {
            var headers = context.Response.Headers;

            headers.TryAdd("X-Content-Type-Options", "nosniff");
            headers.TryAdd("X-Frame-Options", "DENY");
            headers.TryAdd("Referrer-Policy", _options.ReferrerPolicy);
            headers.TryAdd("X-Permitted-Cross-Domain-Policies", "none");
            headers.TryAdd("Cross-Origin-Opener-Policy", "same-origin");
            headers.TryAdd("Cross-Origin-Resource-Policy", "same-origin");
            headers.TryAdd("Permissions-Policy", _options.PermissionsPolicy);
            headers.TryAdd("Content-Security-Policy", _options.ContentSecurityPolicy);

            if (_options.EnableStrictTransportSecurity && !environment.IsDevelopment())
                headers.TryAdd("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
        }

        await next(context);
    }
}
