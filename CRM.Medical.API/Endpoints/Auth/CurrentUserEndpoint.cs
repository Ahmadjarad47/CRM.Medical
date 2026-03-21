using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CRM.Medical.API.Endpoints.Auth.Models;

namespace CRM.Medical.API.Endpoints.Auth;

public static class CurrentUserEndpoint
{
    public static RouteGroupBuilder MapCurrentUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/me", GetCurrentUser)
            .WithName("Auth_GetCurrentUser")
            .WithSummary("Get current authenticated user")
            .WithDescription("Returns the claims-based profile for the currently authenticated principal.")
            .WithTags("Auth")
            .Produces<CurrentUserResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization();

        return group;
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
}
