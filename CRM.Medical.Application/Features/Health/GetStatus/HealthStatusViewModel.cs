namespace CRM.Medical.Application.Features.Health.GetStatus;

public sealed record HealthStatusViewModel(string EnvironmentName, string StatusPlainText, string StatusCssClass);
