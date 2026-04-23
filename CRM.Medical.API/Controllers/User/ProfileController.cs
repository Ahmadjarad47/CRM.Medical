using CRM.Medical.API.Contracts.User.Profile;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Features.Users.Commands.ChangeMyPassword;
using CRM.Medical.Application.Features.Users.Commands.RequestAccountDeletion;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

/// <summary>Authenticated user self-service (caller identity from JWT).</summary>
[Route("api/users/me")]
public sealed class ProfileController(ISender mediator) : UserBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserByIdQuery(User.GetRequiredUserId()), ct));

    [HttpPut]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new UpdateUserCommand(
                User.GetRequiredUserId(),
                request.FullName,
                request.City,
                request.PhoneNumber,
                request.ProfileMetadata),
            ct));

    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await mediator.Send(
            new ChangeMyPasswordCommand(
                User.GetRequiredUserId(),
                request.CurrentPassword,
                request.NewPassword),
            ct);
        return NoContent();
    }

    [HttpPost("request-deletion")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RequestDeletion(CancellationToken ct)
    {
        await mediator.Send(new RequestAccountDeletionCommand(User.GetRequiredUserId()), ct);
        return NoContent();
    }
}
