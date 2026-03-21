using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Features.Auth.Login;
using MediatR;

namespace CRM.Medical.API.Endpoints.Auth;

public static class LoginEndpoint
{
    public static RouteGroupBuilder MapLoginEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/login", LoginAsync)
            .WithName("Auth_Login")
            .WithSummary("Authenticate user")
            .WithDescription("Authenticates a user with email and password and returns a JWT access token.")
            .WithTags("Auth")
            .Accepts<LoginRequest>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .AllowAnonymous();

        return group;
    }

    private static async Task<IResult> LoginAsync(LoginRequest body, ISender mediator)
    {
        var result = await mediator.Send(new LoginCommand(body.Email, body.Password));
        return TypedResults.Ok(result);
    }

    public sealed record LoginRequest(string Email, string Password);
}
