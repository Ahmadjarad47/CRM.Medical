namespace CRM.Medical.API.Contracts.User.Notifications;

public sealed record RegisterDeviceTokenRequest(
    string FcmToken,
    string DeviceType);
