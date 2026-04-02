using CRM.Medical.API.Controllers.User.Models;
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
    // ─── Login / token ────────────────────────────────────────────────────────

    /// <summary>Authenticate with email and password. Returns JWT + refresh token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(request.Email, request.Password), ct);
        return Ok(result);
    }

    /// <summary>Exchange a valid refresh token for a new access + refresh token pair.</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(new RefreshTokenCommand(request.Token), ct);
        return Ok(result);
    }

    // ─── Registration ─────────────────────────────────────────────────────────

    /// <summary>
    /// Self-register a new account. Sends a verification email; login is blocked
    /// until the email is confirmed.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        await mediator.Send(
            new RegisterCommand(
                request.Email,
                request.Password,
                request.FullName,
                request.City,
                request.PhoneNumber,
                request.Role),
            ct);

        return StatusCode(StatusCodes.Status201Created,
            new { message = "Registration successful. Please check your email to verify your account." });
    }

    // ─── Email verification ───────────────────────────────────────────────────

    /// <summary>Confirm email address using the token received by email.</summary>
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new VerifyEmailCommand(request.Email, request.Token), ct);
        return NoContent();
    }

    /// <summary>Resend the email verification link if the previous one expired.</summary>
    [HttpPost("resend-verification")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResendVerification(
        [FromBody] ResendVerificationRequest request,
        CancellationToken ct)
    {
        // Always 204 — never reveal whether an account exists
        await mediator.Send(new ResendEmailVerificationCommand(request.Email), ct);
        return NoContent();
    }

    // ─── Password reset ───────────────────────────────────────────────────────

    /// <summary>
    /// Request a password-reset link. Always returns 204 to prevent user enumeration.
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new ForgotPasswordCommand(request.Email), ct);
        return NoContent();
    }

    /// <summary>Set a new password using the token received by email.</summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken ct)
    {
        await mediator.Send(
            new ResetPasswordCommand(request.Email, request.Token, request.NewPassword), ct);
        return NoContent();
    }

    // ─── Account deletion (confirmation step, no active session required) ─────

    /// <summary>
    /// Permanently deletes an account using the token sent to the user's email
    /// by the authenticated <c>POST /api/users/me/request-deletion</c> endpoint.
    /// </summary>
    [HttpPost("confirm-account-deletion")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmAccountDeletion(
        [FromBody] ConfirmAccountDeletionRequest request,
        CancellationToken ct)
    {
        await mediator.Send(
            new ConfirmAccountDeletionCommand(request.Email, request.Token), ct);
        return NoContent();
    }
}
