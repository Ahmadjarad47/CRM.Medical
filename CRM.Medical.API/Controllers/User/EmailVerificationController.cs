using CRM.Medical.Application.Features.Users.Commands.ConfirmEmail;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.Medical.API.Controllers.User;

[Route("api/users")]
public sealed class EmailVerificationController(ISender sender) : UserBaseController(sender)
{
    [HttpPost("{userId}/confirm-email")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Users_EmailVerification_Confirm",
        Summary = "Confirm user email",
        Description = "Confirms a user's email address using a one-time verification token.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, [FromBody] ConfirmEmailRequest body)
    {
        var command = body.Adapt<ConfirmEmailCommand>() with { UserId = userId };
        await Sender.Send(command);
        return NoContent();
    }

    public sealed record ConfirmEmailRequest(string Token);
}
