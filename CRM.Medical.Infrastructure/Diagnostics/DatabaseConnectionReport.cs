namespace CRM.Medical.Infrastructure.Diagnostics;

public sealed class DatabaseConnectionReport
{
    public bool Verified { get; set; }

    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }
}
