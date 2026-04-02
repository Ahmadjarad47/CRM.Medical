namespace CRM.Medical.API.Middlewares;

public sealed class SecurityHeadersMiddlewareOptions
{
    public bool Enabled { get; init; } = true;
    public bool EnableStrictTransportSecurity { get; init; } = true;
    public string? ContentSecurityPolicy { get; init; }
    public string? ReferrerPolicy { get; init; }
    public string? PermissionsPolicy { get; init; }
}
