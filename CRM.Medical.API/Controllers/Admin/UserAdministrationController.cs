using CRM.Medical.Application.Features.Users.Commands.AddUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.AssignRoles;
using CRM.Medical.Application.Features.Users.Commands.CreateUser;
using CRM.Medical.Application.Features.Users.Commands.LockUser;
using CRM.Medical.Application.Features.Users.Commands.RemoveRoles;
using CRM.Medical.Application.Features.Users.Commands.RemoveUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.SendEmailVerification;
using CRM.Medical.Application.Features.Users.Commands.UnlockUser;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.Commands.UpdateUserPassword;
using CRM.Medical.Application.Features.Users.Commands.UpdateUserPermissions;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Queries.GetAllPermissions;
using CRM.Medical.Application.Features.Users.Queries.GetAllRoles;
using CRM.Medical.Application.Features.Users.Queries.GetUserById;
using CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;
using CRM.Medical.Application.Features.Users.Queries.GetUserRoles;
using CRM.Medical.Application.Features.Users.Queries.GetUsers;
using CRM.Medical.API.Controllers.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.Medical.API.Controllers.Admin;

public sealed class UserAdministrationController(ISender sender) : AdminBaseController(sender)
{
    [HttpPost("")]
    [SwaggerOperation(
        OperationId = "Users_Create",
        Summary = "Create user (Admin)",
        Description = "Creates a new user with selected user type and profile details. Requires Admin role.")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDetailDto>> CreateUserAsync([FromBody] CreateUserRequest body)
    {
        var result = await Sender.Send(body.ToCommand());
        return CreatedAtAction(nameof(GetUserByIdAsync), new { userId = result.Id }, result);
    }

    [HttpGet("")]
    [SwaggerOperation(
        OperationId = "Users_GetAll",
        Summary = "List all users (Admin)",
        Description = "Returns summary profile information for all users. Requires Admin role.")]
    [ProducesResponseType(typeof(IReadOnlyList<UserSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserSummaryDto>>> GetUsersAsync()
    {
        var result = await Sender.Send(new GetUsersQuery());
        return Ok(result);
    }

    [HttpGet("{userId}")]
    [SwaggerOperation(
        OperationId = "Users_GetById",
        Summary = "Get user details by ID (Admin)",
        Description = "Returns detailed profile information for a specific user. Requires Admin role.")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDetailDto>> GetUserByIdAsync(string userId)
    {
        var result = await Sender.Send(new GetUserByIdQuery(userId));
        return Ok(result);
    }

    [HttpPut("{userId}")]
    [SwaggerOperation(
        OperationId = "Users_Profile_Update",
        Summary = "Update user profile (Admin)",
        Description = "Updates core profile fields for the specified user. Requires Admin role.")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDetailDto>> UpdateUserAsync(string userId, [FromBody] UpdateUserRequest body)
    {
        var result = await Sender.Send(body.ToCommand(userId));
        return Ok(result);
    }

    [HttpPut("{userId}/password")]
    [SwaggerOperation(
        OperationId = "Users_Security_UpdatePassword",
        Summary = "Update user password (Admin)",
        Description = "Changes the specified user's password by validating the current password first. Requires Admin role.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserPasswordAsync(string userId, [FromBody] UpdateUserPasswordRequest body)
    {
        await Sender.Send(body.ToCommand(userId));
        return NoContent();
    }

    [HttpGet("roles")]
    [SwaggerOperation(
        OperationId = "Users_Roles_GetAll",
        Summary = "List available roles (Admin)",
        Description = "Returns all role names available for assignment. Requires Admin role.")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetAllRolesAsync()
    {
        var result = await Sender.Send(new GetAllRolesQuery());
        return Ok(result);
    }

    [HttpGet("{userId}/roles")]
    [SwaggerOperation(
        OperationId = "Users_Roles_GetByUserId",
        Summary = "Get assigned roles for a user (Admin)",
        Description = "Returns all roles currently assigned to the specified user. Requires Admin role.")]
    [ProducesResponseType(typeof(UserRolesDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserRolesDto>> GetUserRolesAsync(string userId)
    {
        var result = await Sender.Send(new GetUserRolesQuery(userId));
        return Ok(result);
    }

    [HttpPost("{userId}/roles")]
    [SwaggerOperation(
        OperationId = "Users_Roles_Assign",
        Summary = "Assign roles to a user (Admin)",
        Description = "Assigns one or more roles to the specified user. Requires Admin role.")]
    [ProducesResponseType(typeof(UserRolesDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserRolesDto>> AssignRolesAsync(string userId, [FromBody] ManageRolesRequest body)
    {
        var result = await Sender.Send(body.ToAssignRolesCommand(userId));
        return Ok(result);
    }

    [HttpDelete("{userId}/roles")]
    [SwaggerOperation(
        OperationId = "Users_Roles_Remove",
        Summary = "Remove roles from a user (Admin)",
        Description = "Removes one or more roles from the specified user. Requires Admin role.")]
    [ProducesResponseType(typeof(UserRolesDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserRolesDto>> RemoveRolesAsync(string userId, [FromBody] ManageRolesRequest body)
    {
        var result = await Sender.Send(body.ToRemoveRolesCommand(userId));
        return Ok(result);
    }

    [HttpGet("permissions")]
    [SwaggerOperation(
        OperationId = "Users_Permissions_GetAll",
        Summary = "List available permissions (Admin)",
        Description = "Returns all distinct permission claims known by the system. Requires Admin role.")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetAllPermissionsAsync()
    {
        var result = await Sender.Send(new GetAllPermissionsQuery());
        return Ok(result);
    }

    [HttpGet("{userId}/permissions")]
    [SwaggerOperation(
        OperationId = "Users_Permissions_GetByUserId",
        Summary = "Get assigned permissions for a user (Admin)",
        Description = "Returns all explicit permission claims assigned to the specified user. Requires Admin role.")]
    [ProducesResponseType(typeof(UserPermissionsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserPermissionsDto>> GetUserPermissionsAsync(string userId)
    {
        var result = await Sender.Send(new GetUserPermissionsQuery(userId));
        return Ok(result);
    }

    [HttpPost("{userId}/permissions")]
    [SwaggerOperation(
        OperationId = "Users_Permissions_Add",
        Summary = "Add permissions to a user (Admin)",
        Description = "Adds permission claims to the specified user without removing existing ones. Requires Admin role.")]
    [ProducesResponseType(typeof(UserPermissionsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserPermissionsDto>> AddPermissionsAsync(string userId, [FromBody] ManagePermissionsRequest body)
    {
        var result = await Sender.Send(body.ToAddPermissionsCommand(userId));
        return Ok(result);
    }

    [HttpPut("{userId}/permissions")]
    [SwaggerOperation(
        OperationId = "Users_Permissions_Replace",
        Summary = "Replace user permissions (Admin)",
        Description = "Replaces the current set of user permission claims with the provided set. Requires Admin role.")]
    [ProducesResponseType(typeof(UserPermissionsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserPermissionsDto>> UpdatePermissionsAsync(string userId, [FromBody] ManagePermissionsRequest body)
    {
        var result = await Sender.Send(body.ToUpdatePermissionsCommand(userId));
        return Ok(result);
    }

    [HttpDelete("{userId}/permissions")]
    [SwaggerOperation(
        OperationId = "Users_Permissions_Remove",
        Summary = "Remove permissions from a user (Admin)",
        Description = "Removes specific permission claims from the specified user. Requires Admin role.")]
    [ProducesResponseType(typeof(UserPermissionsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserPermissionsDto>> RemovePermissionsAsync(string userId, [FromBody] ManagePermissionsRequest body)
    {
        var result = await Sender.Send(body.ToRemovePermissionsCommand(userId));
        return Ok(result);
    }

    [HttpPost("{userId}/lock")]
    [SwaggerOperation(
        OperationId = "Users_Security_Lock",
        Summary = "Lock a user account (Admin)",
        Description = "Locks the specified user account for the provided duration in minutes. Requires Admin role.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LockUserAsync(string userId, [FromBody] LockUserRequest body)
    {
        await Sender.Send(body.ToCommand(userId));
        return NoContent();
    }

    [HttpPost("{userId}/unlock")]
    [SwaggerOperation(
        OperationId = "Users_Security_Unlock",
        Summary = "Unlock a user account (Admin)",
        Description = "Removes lockout from the specified user account immediately. Requires Admin role.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnlockUserAsync(string userId)
    {
        await Sender.Send(new UnlockUserCommand(userId));
        return NoContent();
    }

    [HttpPost("{userId}/email-verification")]
    [SwaggerOperation(
        OperationId = "Users_EmailVerification_Send",
        Summary = "Send email verification to a user (Admin)",
        Description = "Sends an email verification message to the specified user. Requires Admin role.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SendEmailVerificationAsync(string userId, [FromBody] SendEmailVerificationRequest body)
    {
        await Sender.Send(body.ToCommand(userId));
        return NoContent();
    }

}
