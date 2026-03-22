namespace CRM.Medical.API.Middlewares;

public sealed class SecurityHeadersMiddlewareOptions
{
    public const string SectionName = "Middlewares:SecurityHeaders";

    public bool Enabled { get; set; } = true;

    public bool EnableStrictTransportSecurity { get; set; } = true;

    public string ContentSecurityPolicy { get; set; } =
        "default-src 'self'; base-uri 'self'; frame-ancestors 'none'; object-src 'none'";

    public string ReferrerPolicy { get; set; } = "no-referrer";

    public string PermissionsPolicy { get; set; } =
        "accelerometer=(), autoplay=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";
}
