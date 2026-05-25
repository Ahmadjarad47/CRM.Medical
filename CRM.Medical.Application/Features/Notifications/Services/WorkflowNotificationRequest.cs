namespace CRM.Medical.Application.Features.Notifications.Services;

public sealed record WorkflowNotificationRequest(
    string EventType,
    IReadOnlyCollection<string> UserIds,
    IReadOnlyDictionary<string, string>? Data = null);
