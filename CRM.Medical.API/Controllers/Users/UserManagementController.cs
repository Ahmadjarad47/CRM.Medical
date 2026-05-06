using CRM.Medical.API.Contracts.Users.UserManagement;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Users.Commands.ActivateUser;
using CRM.Medical.Application.Features.Users.Commands.AssignRoles;
using CRM.Medical.Application.Features.Users.Commands.CreateUser;
using CRM.Medical.Application.Features.Users.Commands.DeactivateUser;
using CRM.Medical.Application.Features.Users.Commands.DeleteUser;
using CRM.Medical.Application.Features.Users.Commands.RemoveRoles;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Queries.GetUserById;
using CRM.Medical.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

/// <summary>
/// User administration endpoints.
/// Admin: all users. Doctor / Lab partner: only users they may manage (see application rules).
/// </summary>
[ApiController]
[Route("api/users/management")]
public sealed class UserManagementController(ISender mediator) : ControllerBase
{
    /// <summary>List users (paged). Requires <c>User:View</c>.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListUsers([FromQuery] ListUsersRequest request, CancellationToken ct) =>
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

    /// <summary>Get one user by id. Requires <c>User:View</c>.</summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(string userId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserByIdQuery(userId), ct));

    /// <summary>Create a user. Requires <c>User:Create</c>.</summary>
    [HttpPost]
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

    /// <summary>Update profile fields and <see cref="UserDetailDto.ProfileMetadata"/>. Requires <c>User:Update</c>.</summary>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateManagedUserRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new UpdateUserCommand(userId, request.FullName, request.City, request.PhoneNumber, request.ProfileMetadata),
            ct));

    /// <summary>Delete a user. Requires <c>User:Delete</c>.</summary>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string userId, CancellationToken ct)
    {
        await mediator.Send(new DeleteUserCommand(userId), ct);
        return NoContent();
    }

    /// <summary>Activate a user. Requires <c>User:Update</c>.</summary>
    [HttpPost("{userId}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(string userId, CancellationToken ct)
    {
        await mediator.Send(new ActivateUserCommand(userId), ct);
        return NoContent();
    }

    /// <summary>Deactivate a user. Requires <c>User:Update</c>.</summary>
    [HttpPost("{userId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(string userId, CancellationToken ct)
    {
        await mediator.Send(new DeactivateUserCommand(userId), ct);
        return NoContent();
    }

    /// <summary>Add roles. Requires <c>User:Update</c>.</summary>
    [HttpPost("{userId}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignRoles(string userId, [FromBody] AssignRolesRequest request, CancellationToken ct)
    {
        await mediator.Send(new AssignRolesCommand(userId, request.Roles), ct);
        return NoContent();
    }

    /// <summary>Remove roles. Requires <c>User:Update</c>.</summary>
    [HttpDelete("{userId}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRoles(string userId, [FromBody] RemoveRolesRequest request, CancellationToken ct)
    {
        await mediator.Send(new RemoveRolesCommand(userId, request.Roles), ct);
        return NoContent();
    }

}
