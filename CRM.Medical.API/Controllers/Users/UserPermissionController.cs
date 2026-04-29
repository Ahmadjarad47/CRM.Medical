using CRM.Medical.API.Contracts.Permissions;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UserPermissionController(ISender mediator, IUserPermissionService userPermissionService)
    : ControllerBase
{
    [HttpGet("{userId}/permissions")]
    [Authorize(Policy = UserPermissions.UsersView)]
    public async Task<IActionResult> GetUserPermissions(string userId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserPermissionsQuery(userId), ct));

    [HttpPost("{userId}/permissions")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Assign(string userId, [FromBody] AssignUserPermissionRequest request, CancellationToken ct)
    {
        await userPermissionService.AssignPermissionToUserAsync(userId, request.PermissionId, ct);
        return NoContent();
    }

    [HttpDelete("{userId}/permissions/{permissionId:guid}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Remove(string userId, Guid permissionId, CancellationToken ct)
    {
        await userPermissionService.RemovePermissionFromUserAsync(userId, permissionId, ct);
        return NoContent();
    }
}
