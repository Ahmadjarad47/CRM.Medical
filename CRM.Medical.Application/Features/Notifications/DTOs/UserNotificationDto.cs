namespace CRM.Medical.Application.Features.Notifications.DTOs;

public sealed record UserNotificationDto(
    int Id,
    string Title,
    string Body,
    IReadOnlyDictionary<string, string>? Data,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? ReadAt);
