using CRM.Medical.API.Controllers.Users.Models;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Users.Commands.ActivateUser;
using CRM.Medical.Application.Features.Users.Commands.AssignPermissions;
using CRM.Medical.Application.Features.Users.Commands.AssignRoles;
using CRM.Medical.Application.Features.Users.Commands.CreateUser;
using CRM.Medical.Application.Features.Users.Commands.DeactivateUser;
using CRM.Medical.Application.Features.Users.Commands.DeleteUser;
using CRM.Medical.Application.Features.Users.Commands.RemovePermission;
using CRM.Medical.Application.Features.Users.Commands.RemoveRoles;
using CRM.Medical.Application.Features.Users.Commands.ReplaceUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Queries.GetUserById;
using CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;
using CRM.Medical.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

/// <summary>
/// User administration for anyone with the corresponding permission claims.
/// Admin: all users. Doctor / Lab partner: only users they may manage (see application rules).
/// </summary>
[ApiController]
[Authorize]
[Route("api/users/management")]
public sealed class UserManagementController(ISender mediator) : ControllerBase
{
    /// <summary>List users (paged). Requires <c>users.view</c>.</summary>
    [HttpGet]
    [Authorize(Policy = UserPermissions.UsersView)]
    [ProducesResponseType(typeof(PagedResult<UserSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListUsers([FromQuery] ListUsersQueryParameters request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new GetUsersQuery(
                request.Page,
                request.PageSize,
                request.Search,
                request.IsActive,
                request.Role,
                request.SortBy,
                request.SortDesc),
            ct));

    /// <summary>Get one user by id. Requires <c>users.view</c>.</summary>
    [HttpGet("{userId}")]
    [Authorize(Policy = UserPermissions.UsersView)]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(string userId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserByIdQuery(userId), ct));

    /// <summary>Create a user. Requires <c>users.create</c>.</summary>
    [HttpPost]
    [Authorize(Policy = UserPermissions.UsersCreate)]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateManagedUserRequest request, CancellationToken ct)
    {
        var user = await mediator.Send(
            new CreateUserCommand(
                request.Email,
                request.FullName,
                request.Password,
                request.City,
                request.PhoneNumber,
                request.Roles ?? [],
                request.ProfileMetadata),
            ct);
        return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
    }

    /// <summary>Update profile fields and <see cref="UserDetailDto.ProfileMetadata"/>. Requires <c>users.update</c>.</summary>
    [HttpPut("{userId}")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateManagedUserRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new UpdateUserCommand(userId, request.FullName, request.City, request.PhoneNumber, request.ProfileMetadata),
            ct));

    /// <summary>Delete a user. Requires <c>users.delete</c>.</summary>
    [HttpDelete("{userId}")]
    [Authorize(Policy = UserPermissions.UsersDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string userId, CancellationToken ct)
    {
        await mediator.Send(new DeleteUserCommand(userId), ct);
        return NoContent();
    }

    /// <summary>Activate a user. Requires <c>users.update</c>.</summary>
    [HttpPost("{userId}/activate")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(string userId, CancellationToken ct)
    {
        await mediator.Send(new ActivateUserCommand(userId), ct);
        return NoContent();
    }

    /// <summary>Deactivate a user. Requires <c>users.update</c>.</summary>
    [HttpPost("{userId}/deactivate")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(string userId, CancellationToken ct)
    {
        await mediator.Send(new DeactivateUserCommand(userId), ct);
        return NoContent();
    }

    /// <summary>Add roles. Requires <c>users.update</c>.</summary>
    [HttpPost("{userId}/roles")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignRoles(string userId, [FromBody] AssignRolesBody request, CancellationToken ct)
    {
        await mediator.Send(new AssignRolesCommand(userId, request.Roles), ct);
        return NoContent();
    }

    /// <summary>Remove roles. Requires <c>users.update</c>.</summary>
    [HttpDelete("{userId}/roles")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRoles(string userId, [FromBody] RemoveRolesBody request, CancellationToken ct)
    {
        await mediator.Send(new RemoveRolesCommand(userId, request.Roles), ct);
        return NoContent();
    }

    /// <summary>List effective permission claims for a user. Requires <c>users.view</c>.</summary>
    [HttpGet("{userId}/permissions")]
    [Authorize(Policy = UserPermissions.UsersView)]
    [ProducesResponseType(typeof(UserPermissionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserPermissions(string userId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserPermissionsQuery(userId), ct));

    /// <summary>Add permission claims. Requires <c>users.manage_permissions</c>.</summary>
    [HttpPost("{userId}/permissions")]
    [Authorize(Policy = UserPermissions.UsersManagePermissions)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignPermissions(string userId, [FromBody] AssignPermissionsBody request, CancellationToken ct)
    {
        await mediator.Send(new AssignPermissionsToUserCommand(userId, request.Permissions), ct);
        return NoContent();
    }

    /// <summary>Replace all permission claims. Requires <c>users.manage_permissions</c>.</summary>
    [HttpPut("{userId}/permissions")]
    [Authorize(Policy = UserPermissions.UsersManagePermissions)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ReplacePermissions(string userId, [FromBody] ReplacePermissionsBody request, CancellationToken ct)
    {
        await mediator.Send(new ReplaceUserPermissionsCommand(userId, request.Permissions ?? []), ct);
        return NoContent();
    }

    /// <summary>Remove one permission claim. Requires <c>users.manage_permissions</c>.</summary>
    [HttpDelete("{userId}/permissions/{permission}")]
    [Authorize(Policy = UserPermissions.UsersManagePermissions)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemovePermission(string userId, string permission, CancellationToken ct)
    {
        await mediator.Send(new RemovePermissionFromUserCommand(userId, permission), ct);
        return NoContent();
    }
}
