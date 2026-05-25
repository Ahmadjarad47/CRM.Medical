using CRM.Medical.API.Contracts.User.Notifications;
using CRM.Medical.Application.Abstractions;
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
}
