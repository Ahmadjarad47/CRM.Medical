namespace CRM.Medical.Application.Health;

public sealed record DatabaseHealthSnapshot(bool Verified, bool Success, string? ErrorMessage);
