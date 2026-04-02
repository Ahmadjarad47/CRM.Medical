namespace CRM.Medical.Application.Features.Health.GetStatus;

public sealed record HealthStatusViewModel(
    string Status,
    string Environment,
    DateTime Timestamp,
    DatabaseStatus Database);

public sealed record DatabaseStatus(
    bool IsHealthy,
    string? ErrorMessage,
    DateTime LastChecked);
