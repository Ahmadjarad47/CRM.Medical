using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CRM.Medical.API.Controllers.User.Models;
using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Features.Auth.Login;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.Medical.API.Controllers.User;

[Route("api/auth")]
public sealed class AuthController(ISender sender) : UserBaseController(sender)
{
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Auth_Login",
        Summary = "Authenticate user",
        Description = "Authenticates a user with email and password and returns a JWT access token.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest body)
    {
        var command = body.Adapt<LoginCommand>();
        var result = await Sender.Send(command);
        return Ok(result);
    }

    [HttpGet("me")]
    [SwaggerOperation(
        OperationId = "Auth_GetCurrentUser",
        Summary = "Get current authenticated user",
        Description = "Returns the claims-based profile for the currently authenticated principal.")]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    public ActionResult<CurrentUserResponse> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? string.Empty;
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? User.FindFirstValue(ClaimTypes.Email);

        var response = new CurrentUserMappingModel(userId, email).Adapt<CurrentUserResponse>();
        return Ok(response);
    }

    private sealed record CurrentUserMappingModel(string UserId, string? Email);

    public sealed record LoginRequest(string Email, string Password);
}
