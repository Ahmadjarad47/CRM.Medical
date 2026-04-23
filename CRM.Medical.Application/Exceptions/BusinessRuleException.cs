namespace CRM.Medical.Application.Exceptions;

/// <summary>
/// A predictable business rule violation (HTTP 400). The client can correct the request.
/// </summary>
public sealed class BusinessRuleException : ApplicationExceptionBase
{
    public BusinessRuleException(string message, string detail)
        : base(detail)
    {
        PublicMessage = message;
    }

    public override int StatusCode => 400;

    /// <summary>High-level, stable category (e.g. "Invalid banner data").</summary>
    public string PublicMessage { get; }

    public string Detail => Message;
}
