namespace CRM.Medical.Application.Features.Notifications.Services;

public interface INotificationService
{
    Task SaveDeviceTokenAsync(
        string userId,
        NotificationDeviceTokenUpsertRequest request,
        CancellationToken cancellationToken);

    Task RemoveDeviceTokenAsync(
        string userId,
        string fcmToken,
        CancellationToken cancellationToken);

    Task SendToUserAsync(
        string userId,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken);

    Task SendToUsersAsync(
        IReadOnlyCollection<string> userIds,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken);

    Task SendWorkflowNotificationAsync(
        WorkflowNotificationRequest request,
        CancellationToken cancellationToken);
}
