using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
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

    private static async Task<IResult> LoginAsync(LoginCommand body, ISender mediator)
    {
        var result = await mediator.Send(body);
        return result is null ? Results.Unauthorized() : TypedResults.Ok(result);
    }
}
