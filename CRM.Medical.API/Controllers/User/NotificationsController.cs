using CRM.Medical.API.Contracts.User.Notifications;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Notifications.DTOs;
using CRM.Medical.Application.Features.Notifications.Services;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Controllers.User;

[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Doctor + "," + UserRoles.Patient + "," + UserRoles.LabPartner + "," + UserRoles.User)]
[ApiController]
[Route("api/notifications")]
public sealed class NotificationsController(
    INotificationService notificationService,
    ICurrentUserAccessor currentUser) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserNotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListMyNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await notificationService.GetUserNotificationsAsync(
            currentUser.UserId ?? throw new InvalidOperationException("Current user id was not found."),
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("device-tokens")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SaveDeviceToken(
        [FromBody] RegisterDeviceTokenRequest request,
        CancellationToken cancellationToken)
    {
        await notificationService.SaveDeviceTokenAsync(
            currentUser.UserId ?? throw new InvalidOperationException("Current user id was not found."),
            new NotificationDeviceTokenUpsertRequest(request.FcmToken, request.DeviceType),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("device-tokens")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveDeviceToken(
        [FromBody] RemoveDeviceTokenRequest request,
        CancellationToken cancellationToken)
    {
        await notificationService.RemoveDeviceTokenAsync(
            currentUser.UserId ?? throw new InvalidOperationException("Current user id was not found."),
            request.FcmToken,
            cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendAdminNotification(
        [FromBody] SendAdminNotificationRequest request,
        CancellationToken cancellationToken)
    {
        await notificationService.SendAdminNotificationAsync(
            new AdminNotificationRequest(
                request.Title,
                request.Body,
                request.TargetUserId,
                request.TargetRole,
                request.Data),
            cancellationToken);
        return NoContent();
    }
}
