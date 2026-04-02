using Microsoft.Extensions.Options;

namespace CRM.Medical.API.Middlewares;

public sealed class SecurityHeadersMiddleware(
    RequestDelegate next,
    IOptions<SecurityHeadersMiddlewareOptions> options)
{
    private readonly SecurityHeadersMiddlewareOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.Enabled)
        {
            var headers = context.Response.Headers;

            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["X-XSS-Protection"] = "0"; // Modern browsers: disable legacy XSS filter

            if (_options.EnableStrictTransportSecurity)
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

            if (!string.IsNullOrEmpty(_options.ContentSecurityPolicy))
                headers["Content-Security-Policy"] = _options.ContentSecurityPolicy;

            if (!string.IsNullOrEmpty(_options.ReferrerPolicy))
                headers["Referrer-Policy"] = _options.ReferrerPolicy;

            if (!string.IsNullOrEmpty(_options.PermissionsPolicy))
                headers["Permissions-Policy"] = _options.PermissionsPolicy;
        }

        await next(context);
    }
}
