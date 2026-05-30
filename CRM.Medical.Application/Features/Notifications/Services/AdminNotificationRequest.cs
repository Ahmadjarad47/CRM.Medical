namespace CRM.Medical.Application.Features.Notifications.Services;

public sealed record AdminNotificationRequest(
    string Title,
    string Body,
    string? TargetUserId = null,
    string? TargetRole = null,
    IReadOnlyDictionary<string, string>? Data = null);
