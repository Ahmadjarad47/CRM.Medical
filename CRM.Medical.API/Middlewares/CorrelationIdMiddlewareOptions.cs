namespace CRM.Medical.API.Middlewares;

public sealed class CorrelationIdMiddlewareOptions
{
    public const string SectionName = "Middlewares:CorrelationId";

    public bool Enabled { get; set; } = true;

    public string HeaderName { get; set; } = "X-Correlation-ID";

    public bool IncludeInResponse { get; set; } = true;

    public bool UseTraceIdentifierWhenMissing { get; set; } = true;
}
