using CRM.Medical.API.Contracts.User.Auth;
using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Features.Auth.ForgotPassword;
using CRM.Medical.Application.Features.Auth.Login;
using CRM.Medical.Application.Features.Auth.RefreshToken;
using CRM.Medical.Application.Features.Auth.Register;
using CRM.Medical.Application.Features.Auth.ResendEmailVerification;
using CRM.Medical.Application.Features.Auth.ResetPassword;
using CRM.Medical.Application.Features.Auth.VerifyEmail;
using CRM.Medical.Application.Features.Users.Commands.ConfirmAccountDeletion;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController(ISender mediator) : UserBaseController
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(new LoginCommand(request.Email, request.Password), ct));

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(new RefreshTokenCommand(request.Token), ct));

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new RegisterCommand(
                request.Email,
                request.Password,
                request.FullName,
                request.City,
                request.PhoneNumber,
                request.Role),
            ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken ct)
    {
        await mediator.Send(new VerifyEmailCommand(request.Email, request.Token), ct);
        return NoContent();
    }

    [HttpPost("resend-verification")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request, CancellationToken ct)
    {
        await mediator.Send(new ResendEmailVerificationCommand(request.Email), ct);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        await mediator.Send(new ForgotPasswordCommand(request.Email), ct);
        return NoContent();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        await mediator.Send(
            new ResetPasswordCommand(request.Email, request.Token, request.NewPassword),
            ct);
        return NoContent();
    }

    [HttpPost("confirm-account-deletion")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmAccountDeletion([FromBody] ConfirmAccountDeletionRequest request, CancellationToken ct)
    {
        await mediator.Send(new ConfirmAccountDeletionCommand(request.Email, request.Token), ct);
        return NoContent();
    }
}
