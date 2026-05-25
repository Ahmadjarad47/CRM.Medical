namespace CRM.Medical.Application.Features.Notifications.Services;

public sealed record FirebasePushSendResult(
    string Token,
    bool IsSuccess,
    bool ShouldDeactivateToken,
    string? ErrorCode);
