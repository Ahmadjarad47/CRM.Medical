using CRM.Medical.API.Controllers.User.Models;
using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Features.Auth.ForgotPassword;
using CRM.Medical.Application.Features.Auth.Login;
using CRM.Medical.Application.Features.Auth.RefreshToken;
using CRM.Medical.Application.Features.Auth.ResendEmailVerification;
using CRM.Medical.Application.Features.Auth.ResetPassword;
using CRM.Medical.Application.Features.Users.Commands.ConfirmEmail;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.Medical.API.Controllers.User;

[Route("api/auth")]
public sealed class AuthController(ISender sender) : UserBaseController(sender)
{
    [HttpPost("create-account")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Auth_CreateAccount",
        Summary = "Create account",
        Description = "Registers a new patient account and sends an email verification link.")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDetailDto>> CreateAccountAsync(
        [FromBody] CreateAccountRequest body)
    {
        var created = await Sender.Send(body.ToCreateAccountCommand());
        await Sender.Send(
            new ResendEmailVerificationCommand(
                created.Email,
                body.ConfirmationBaseUrl));

        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Auth_Login",
        Summary = "Authenticate user",
        Description = "Authenticates a user with email and password and returns access and refresh tokens.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest body)
    {
        var result = await Sender.Send(body.ToCommand());
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Auth_RefreshToken",
        Summary = "Refresh access token",
        Description = "Exchanges a valid refresh token for a new access token and rotates refresh token.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> RefreshTokenAsync([FromBody] RefreshTokenRequest body)
    {
        var result = await Sender.Send(body.ToCommand());
        return Ok(result);
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Auth_ConfirmEmail",
        Summary = "Confirm user email",
        Description = "Confirms a user's email address using a one-time verification token.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest body)
    {
        await Sender.Send(body.ToCommand());
        return NoContent();
    }

    [HttpPost("resend-email-verification")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Auth_ResendEmailVerification",
        Summary = "Resend verification email",
        Description = "Sends email verification link if account exists and is not yet confirmed.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResendEmailVerificationAsync([FromBody] ResendEmailVerificationRequest body)
    {
        await Sender.Send(body.ToCommand());
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Auth_ForgotPassword",
        Summary = "Request password reset",
        Description = "Sends password reset link if account exists and is eligible.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest body)
    {
        await Sender.Send(body.ToCommand());
        return NoContent();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Auth_ResetPassword",
        Summary = "Reset password",
        Description = "Resets password using one-time password reset token.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest body)
    {
        await Sender.Send(body.ToCommand());
        return NoContent();
    }

    [HttpGet("profile")]
    [SwaggerOperation(
        OperationId = "Auth_GetProfile",
        Summary = "Get authenticated user profile",
        Description = "Returns full profile details for currently authenticated user.")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDetailDto>> GetProfileAsync()
    {
        var userId = GetRequiredUserId();
        var result = await Sender.Send(new GetUserByIdQuery(userId));
        return Ok(result);
    }

    [HttpPut("profile")]
    [SwaggerOperation(
        OperationId = "Auth_UpdateProfile",
        Summary = "Update authenticated user profile",
        Description = "Updates profile fields of currently authenticated user while preserving account type and security fields.")]
    [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDetailDto>> UpdateProfileAsync([FromBody] UpdateProfileRequest body)
    {
        var userId = GetRequiredUserId();
        var current = await Sender.Send(new GetUserByIdQuery(userId));
        var updated = await Sender.Send(body.ToUpdateProfileCommand(userId, current));
        return Ok(updated);
    }

    [HttpGet("me")]
    [SwaggerOperation(
        OperationId = "Auth_GetCurrentUser",
        Summary = "Get current authenticated user",
        Description = "Returns minimal claims-based identity details for currently authenticated principal.")]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    public ActionResult<CurrentUserResponse> GetCurrentUser() =>
        Ok(new CurrentUserResponse(GetRequiredUserId(), GetCurrentUserEmail()));
}
