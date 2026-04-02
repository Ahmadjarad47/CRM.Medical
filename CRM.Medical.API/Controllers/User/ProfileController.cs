using System.Security.Claims;
using System.Text.Json;
using CRM.Medical.API.Controllers.User.Models;
using CRM.Medical.Application.Features.Users.Commands.ChangeMyPassword;
using CRM.Medical.Application.Features.Users.Commands.RequestAccountDeletion;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

/// <summary>
/// Authenticated user self-service: view and manage their own profile.
/// All endpoints operate on the caller's identity (extracted from the JWT sub claim).
/// </summary>
[Route("api/users/me")]
public sealed class ProfileController(ISender mediator) : UserBaseController
{
    // ─── Helpers ──────────────────────────────────────────────────────────────

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Unable to identify the current user.");

    // ─── Profile ──────────────────────────────────────────────────────────────

    /// <summary>Returns the current user's full profile including roles and permissions.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByIdQuery(CurrentUserId), ct);
        return Ok(result);
    }

    /// <summary>Updates display name, city, phone number and profile metadata (JSONB).</summary>
    [HttpPut]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken ct)
    {
        JsonElement? metadata = request.ProfileMetadata is null
            ? null
            : JsonSerializer.SerializeToElement(request.ProfileMetadata);

        var result = await mediator.Send(
            new UpdateUserCommand(
                CurrentUserId,
                request.FullName,
                request.City,
                request.PhoneNumber,
                metadata),
            ct);

        return Ok(result);
    }

    // ─── Password ─────────────────────────────────────────────────────────────

    /// <summary>Changes the current user's password. Requires the existing password.</summary>
    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct)
    {
        await mediator.Send(
            new ChangeMyPasswordCommand(
                CurrentUserId,
                request.CurrentPassword,
                request.NewPassword),
            ct);

        return NoContent();
    }

    // ─── Account deletion ─────────────────────────────────────────────────────

    /// <summary>
    /// Sends a confirmation email with a deletion token.
    /// The user must then call <c>POST /api/auth/confirm-account-deletion</c>
    /// with that token to permanently delete their account.
    /// </summary>
    [HttpPost("request-deletion")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RequestDeletion(CancellationToken ct)
    {
        await mediator.Send(new RequestAccountDeletionCommand(CurrentUserId), ct);
        return NoContent();
    }
}
