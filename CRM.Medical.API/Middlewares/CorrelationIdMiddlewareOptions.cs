namespace CRM.Medical.API.Middlewares;

public sealed class CorrelationIdMiddlewareOptions
{
    public bool Enabled { get; init; } = true;
    public string HeaderName { get; init; } = "X-Correlation-ID";
    public bool IncludeInResponse { get; init; } = true;
    public bool UseTraceIdentifierWhenMissing { get; init; } = true;
}
