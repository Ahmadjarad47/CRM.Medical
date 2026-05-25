using CRM.Medical.Application.Features.Notifications.Services;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Notifications;

internal sealed class NotificationService(
    MedicalDbContext db,
    IFirebasePushSender firebasePushSender,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task SaveDeviceTokenAsync(
        string userId,
        NotificationDeviceTokenUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedToken = request.FcmToken.Trim();
        var normalizedDeviceType = request.DeviceType.Trim();
        if (string.IsNullOrWhiteSpace(normalizedToken))
            throw new ApplicationBadRequestException("FcmToken is required.");

        var deviceType = NormalizeDeviceType(normalizedDeviceType)
            ?? throw new ApplicationBadRequestException("DeviceType must be one of: Web, Android, iOS.");

        var existing = await db.UserDeviceTokens
            .FirstOrDefaultAsync(x => x.FcmToken == normalizedToken, cancellationToken);

        if (existing is null)
        {
            db.UserDeviceTokens.Add(new UserDeviceToken
            {
                UserId = userId,
                FcmToken = normalizedToken,
                DeviceType = deviceType,
                IsActive = true
            });
        }
        else
        {
            existing.UserId = userId;
            existing.DeviceType = deviceType;
            existing.IsActive = true;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveDeviceTokenAsync(string userId, string fcmToken, CancellationToken cancellationToken)
    {
        var normalizedToken = fcmToken.Trim();
        if (string.IsNullOrWhiteSpace(normalizedToken))
            return;

        var tokenRows = await db.UserDeviceTokens
            .Where(x => x.UserId == userId && x.FcmToken == normalizedToken)
            .ToListAsync(cancellationToken);

        if (tokenRows.Count == 0)
            return;

        foreach (var row in tokenRows)
            row.IsActive = false;

        await db.SaveChangesAsync(cancellationToken);
    }

    public Task SendToUserAsync(
        string userId,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken) =>
        SendToUsersAsync([userId], title, body, data, cancellationToken);

    public async Task SendToUsersAsync(
        IReadOnlyCollection<string> userIds,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        try
        {
            var normalizedUserIds = userIds
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (normalizedUserIds.Length == 0)
                return;

            var tokenRows = await db.UserDeviceTokens
                .Where(x => normalizedUserIds.Contains(x.UserId) && x.IsActive)
                .Select(x => new { x.Id, x.FcmToken })
                .ToListAsync(cancellationToken);

            var tokens = tokenRows
                .Select(x => x.FcmToken)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (tokens.Count == 0)
                return;

            var sendResults = await firebasePushSender.SendAsync(tokens, title, body, data, cancellationToken);
            var deactivatedTokens = sendResults
                .Where(x => x.ShouldDeactivateToken)
                .Select(x => x.Token)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (deactivatedTokens.Length > 0)
            {
                var staleTokens = await db.UserDeviceTokens
                    .Where(x => deactivatedTokens.Contains(x.FcmToken))
                    .ToListAsync(cancellationToken);

                foreach (var staleToken in staleTokens)
                    staleToken.IsActive = false;

                await db.SaveChangesAsync(cancellationToken);
            }

            var failedCount = sendResults.Count(x => !x.IsSuccess);
            if (failedCount > 0)
            {
                logger.LogWarning(
                    "Firebase push completed with failures. Failed={FailedCount}, Total={TotalCount}",
                    failedCount,
                    sendResults.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Push notification delivery failed. Workflow will continue.");
        }
    }

    public Task SendWorkflowNotificationAsync(
        WorkflowNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var template = NotificationTemplates.Build(request);
        return SendToUsersAsync(request.UserIds, template.Title, template.Body, request.Data, cancellationToken);
    }

    private static string? NormalizeDeviceType(string deviceType) =>
        deviceType switch
        {
            var value when value.Equals("Web", StringComparison.OrdinalIgnoreCase) => "Web",
            var value when value.Equals("Android", StringComparison.OrdinalIgnoreCase) => "Android",
            var value when value.Equals("iOS", StringComparison.OrdinalIgnoreCase) => "iOS",
            _ => null
        };
}
