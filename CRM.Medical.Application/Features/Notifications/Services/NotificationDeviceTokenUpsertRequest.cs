namespace CRM.Medical.Application.Features.Notifications.Services;

public sealed record NotificationDeviceTokenUpsertRequest(
    string FcmToken,
    string DeviceType);
