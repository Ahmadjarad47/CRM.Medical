using System.Text.Json;
using CRM.Medical.API.Controllers.Admin.Models;
using CRM.Medical.Application.Features.Users.Commands.AssignPermissions;
using CRM.Medical.Application.Features.Users.Commands.AssignRoles;
using CRM.Medical.Application.Features.Users.Commands.ActivateUser;
using CRM.Medical.Application.Features.Users.Commands.CreateUser;
using CRM.Medical.Application.Features.Users.Commands.DeactivateUser;
using CRM.Medical.Application.Features.Users.Commands.DeleteUser;
using CRM.Medical.Application.Features.Users.Commands.RemovePermission;
using CRM.Medical.Application.Features.Users.Commands.RemoveRoles;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.Queries.GetUserById;
using CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;
using CRM.Medical.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/users")]
public sealed class UsersController(ISender mediator) : AdminBaseController
{
    // ─── User list & detail ───────────────────────────────────────────────────

    [HttpGet]
    [Authorize(Policy = UserPermissions.UsersView)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetUsersQuery(
                request.Page,
                request.PageSize,
                request.Search,
                request.IsActive,
                request.Role,
                request.SortBy,
                request.SortDesc),
            ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = UserPermissions.UsersView)]
    public async Task<IActionResult> GetUserById(string id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), ct);
        return Ok(result);
    }

    // ─── Create / update / delete ─────────────────────────────────────────────

    [HttpPost]
    [Authorize(Policy = UserPermissions.UsersCreate)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        JsonElement? metadata = request.ProfileMetadata is null
            ? null
            : JsonSerializer.SerializeToElement(request.ProfileMetadata);

        var command = new CreateUserCommand(
            request.Email,
            request.FullName,
            request.Password,
            request.City,
            request.PhoneNumber,
            request.Roles ?? [],
            metadata);

        var user = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    public async Task<IActionResult> UpdateUser(
        string id,
        [FromBody] UpdateUserRequest request,
        CancellationToken ct)
    {
        JsonElement? metadata = request.ProfileMetadata is null
            ? null
            : JsonSerializer.SerializeToElement(request.ProfileMetadata);

        var result = await mediator.Send(
            new UpdateUserCommand(id, request.FullName, request.City, request.PhoneNumber, metadata), ct);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = UserPermissions.UsersDelete)]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken ct)
    {
        await mediator.Send(new DeleteUserCommand(id), ct);
        return NoContent();
    }

    // ─── Activate / deactivate ────────────────────────────────────────────────

    [HttpPost("{id}/activate")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    public async Task<IActionResult> ActivateUser(string id, CancellationToken ct)
    {
        await mediator.Send(new ActivateUserCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    public async Task<IActionResult> DeactivateUser(string id, CancellationToken ct)
    {
        await mediator.Send(new DeactivateUserCommand(id), ct);
        return NoContent();
    }

    // ─── Role management (classification only) ────────────────────────────────

    [HttpPost("{id}/roles")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    public async Task<IActionResult> AssignRoles(
        string id,
        [FromBody] AssignRolesRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new AssignRolesCommand(id, request.Roles), ct);
        return NoContent();
    }

    [HttpDelete("{id}/roles")]
    [Authorize(Policy = UserPermissions.UsersUpdate)]
    public async Task<IActionResult> RemoveRoles(
        string id,
        [FromBody] RemoveRolesRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new RemoveRolesCommand(id, request.Roles), ct);
        return NoContent();
    }

    // ─── Permission management ────────────────────────────────────────────────

    /// <summary>Returns all permission claims assigned to the user.</summary>
    [HttpGet("{id}/permissions")]
    [Authorize(Policy = UserPermissions.UsersView)]
    public async Task<IActionResult> GetUserPermissions(string id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserPermissionsQuery(id), ct);
        return Ok(result);
    }

    /// <summary>Assigns one or more permissions to a user (idempotent).</summary>
    [HttpPost("{id}/permissions")]
    [Authorize(Policy = UserPermissions.UsersManagePermissions)]
    public async Task<IActionResult> AssignPermissions(
        string id,
        [FromBody] AssignPermissionsRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new AssignPermissionsToUserCommand(id, request.Permissions), ct);
        return NoContent();
    }

    /// <summary>Removes a single named permission from a user (idempotent).</summary>
    [HttpDelete("{id}/permissions/{permission}")]
    [Authorize(Policy = UserPermissions.UsersManagePermissions)]
    public async Task<IActionResult> RemovePermission(
        string id,
        string permission,
        CancellationToken ct)
    {
        await mediator.Send(new RemovePermissionFromUserCommand(id, permission), ct);
        return NoContent();
    }
}
