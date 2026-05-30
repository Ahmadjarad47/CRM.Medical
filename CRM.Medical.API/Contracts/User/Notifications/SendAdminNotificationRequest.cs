namespace CRM.Medical.API.Contracts.User.Notifications;

public sealed record SendAdminNotificationRequest(
    string Title,
    string Body,
    string? TargetUserId = null,
    string? TargetRole = null,
    IReadOnlyDictionary<string, string>? Data = null);
