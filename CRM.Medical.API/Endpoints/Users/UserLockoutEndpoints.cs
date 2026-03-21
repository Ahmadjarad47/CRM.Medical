using CRM.Medical.Application.Features.Users.Commands.LockUser;
using CRM.Medical.Application.Features.Users.Commands.UnlockUser;
using MediatR;

namespace CRM.Medical.API.Endpoints.Users;

public static class UserLockoutEndpoints
{
    private const string AdminRole = "Admin";

    public static RouteGroupBuilder MapUserLockoutEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{userId:guid}/lock", LockUserAsync)
            .WithName("Users_Security_Lock")
            .WithSummary("Lock a user account (Admin)")
            .WithDescription("Locks the specified user account for the provided duration in minutes. Requires Admin role.")
            .WithTags("User Security")
            .Accepts<LockUserRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapPost("/{userId:guid}/unlock", UnlockUserAsync)
            .WithName("Users_Security_Unlock")
            .WithSummary("Unlock a user account (Admin)")
            .WithDescription("Removes lockout from the specified user account immediately. Requires Admin role.")
            .WithTags("User Security")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        return group;
    }

    private static async Task<IResult> LockUserAsync(string userId, LockUserRequest body, ISender mediator)
    {
        await mediator.Send(new LockUserCommand(userId, body.LockoutMinutes));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> UnlockUserAsync(string userId, ISender mediator)
    {
        await mediator.Send(new UnlockUserCommand(userId));
        return TypedResults.NoContent();
    }

    public sealed record LockUserRequest(int LockoutMinutes);
}
