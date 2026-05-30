using System.Text.Json;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Notifications.DTOs;
using CRM.Medical.Application.Features.Notifications.Services;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
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
    public async Task<PagedResult<UserNotificationDto>> GetUserNotificationsAsync(
        string userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedUserId = userId.Trim();
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);

        var query = db.UserNotifications
            .AsNoTracking()
            .Where(x => x.UserId == normalizedUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var notificationRows = await query
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserNotificationDto>
        {
            Items = notificationRows.Select(MapNotificationDto).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

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

    public async Task SendAdminNotificationAsync(
        AdminNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var title = request.Title?.Trim() ?? string.Empty;
        var body = request.Body?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
            throw new ApplicationBadRequestException("Title is required.");

        if (string.IsNullOrWhiteSpace(body))
            throw new ApplicationBadRequestException("Body is required.");

        var targetUserId = request.TargetUserId?.Trim();
        var targetRole = NormalizeAdminNotificationRole(request.TargetRole);

        if (!string.IsNullOrWhiteSpace(targetUserId) && targetRole is not null)
            throw new ApplicationBadRequestException("TargetUserId and TargetRole cannot be used together.");

        var targetUserIdsQuery = db.Users
            .Where(x => x.IsActive)
            .Select(x => x.Id);

        if (!string.IsNullOrWhiteSpace(targetUserId))
        {
            targetUserIdsQuery = targetUserIdsQuery
                .Where(x => x == targetUserId);
        }
        else if (targetRole is not null)
        {
            var normalizedRoleName = targetRole.ToUpperInvariant();
            var roleUserIds =
                from userRole in db.UserRoles
                join role in db.Roles on userRole.RoleId equals role.Id
                where role.NormalizedName == normalizedRoleName
                select userRole.UserId;

            targetUserIdsQuery = targetUserIdsQuery
                .Where(x => roleUserIds.Contains(x));
        }

        var targetUserIds = await targetUserIdsQuery
            .Distinct()
            .ToListAsync(cancellationToken);

        if (targetUserIds.Count == 0)
            return;

        await PersistUserNotificationsAsync(targetUserIds, title, body, request.Data, cancellationToken);
        await SendPushToUserIdsAsync(targetUserIds, title, body, request.Data, cancellationToken);
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

            await PersistUserNotificationsAsync(normalizedUserIds, title, body, data, cancellationToken);
            await SendPushToUserIdsAsync(normalizedUserIds, title, body, data, cancellationToken);
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

    private static string? NormalizeAdminNotificationRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return null;

        var normalizedRole = role.Trim();
        return normalizedRole switch
        {
            var value when value.Equals(UserRoles.Doctor, StringComparison.OrdinalIgnoreCase) => UserRoles.Doctor,
            var value when value.Equals(UserRoles.LabPartner, StringComparison.OrdinalIgnoreCase) => UserRoles.LabPartner,
            var value when value.Equals(UserRoles.Patient, StringComparison.OrdinalIgnoreCase) => UserRoles.Patient,
            _ => throw new ApplicationBadRequestException(
                "TargetRole must be one of: Doctor, LabPartner, Patient, or omitted for all users.")
        };
    }

    private async Task PersistUserNotificationsAsync(
        IReadOnlyCollection<string> userIds,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        var normalizedUserIds = userIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (normalizedUserIds.Length == 0)
            return;

        foreach (var userId in normalizedUserIds)
        {
            db.UserNotifications.Add(new UserNotification
            {
                UserId = userId,
                Title = title,
                Body = body,
                Data = CreateDataDocument(data)
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task SendPushToUserIdsAsync(
        IReadOnlyCollection<string> userIds,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        var normalizedUserIds = userIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (normalizedUserIds.Length == 0)
            return;

        var tokens = await db.UserDeviceTokens
            .Where(x => normalizedUserIds.Contains(x.UserId) && x.IsActive)
            .Select(x => x.FcmToken)
            .ToListAsync(cancellationToken);

        await SendToTokensAsync(tokens, title, body, data, cancellationToken);
    }

    private async Task SendToTokensAsync(
        IReadOnlyCollection<string> tokens,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        var normalizedTokens = tokens
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (normalizedTokens.Count == 0)
            return;

        var sendResults = await firebasePushSender.SendAsync(normalizedTokens, title, body, data, cancellationToken);
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

    private static UserNotificationDto MapNotificationDto(UserNotification notification) =>
        new(
            notification.Id,
            notification.Title,
            notification.Body,
            ToStringDictionary(notification.Data),
            notification.IsRead,
            notification.CreatedAt,
            notification.ReadAt);

    private static JsonDocument? CreateDataDocument(IReadOnlyDictionary<string, string>? data)
    {
        if (data is null || data.Count == 0)
            return null;

        return JsonDocument.Parse(JsonSerializer.Serialize(data));
    }

    private static IReadOnlyDictionary<string, string>? ToStringDictionary(JsonDocument? document)
    {
        if (document is null)
            return null;

        return JsonSerializer.Deserialize<Dictionary<string, string>>(document.RootElement.GetRawText());
    }
}
