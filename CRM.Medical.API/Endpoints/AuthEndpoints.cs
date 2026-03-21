using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithTags("Auth")
            .WithSummary("Test JWT authorization")
            .WithDescription("Requires a valid Bearer token from POST /api/auth/login. Returns the user id and email from the token.")
            .Produces<CurrentUserResponse>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        app.MapPost("/api/auth/login", LoginAsync)
            .WithName("Login")
            .WithTags("Auth")
            .WithSummary("Sign in with email and password")
            .WithDescription(
                "Returns a JWT when credentials are valid. " +
                "Invalid body fields return 400 with validation errors; wrong credentials return 401.")
            .Accepts<LoginCommand>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK, "application/json")
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

        return app;
    }

    private static CurrentUserResponse GetCurrentUser(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? string.Empty;
        var email = user.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? user.FindFirstValue(ClaimTypes.Email);
        return new CurrentUserResponse(userId, email);
    }

    private static async Task<IResult> LoginAsync(LoginCommand body, ISender mediator)
    {
        var result = await mediator.Send(body);
        return result is null ? Results.Unauthorized() : TypedResults.Ok(result);
    }
}
