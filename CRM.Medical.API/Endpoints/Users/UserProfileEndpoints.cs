using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.Commands.UpdateUserPassword;
using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.API.Endpoints.Users;

public static class UserProfileEndpoints
{
    private const string AdminRole = "Admin";

    public static RouteGroupBuilder MapUserProfileEndpoints(this RouteGroupBuilder group)
    {
        group.MapPut("/{userId:guid}", UpdateUserAsync)
            .WithName("Users_Profile_Update")
            .WithSummary("Update user profile (Admin)")
            .WithDescription("Updates core profile fields for the specified user. Requires Admin role.")
            .WithTags("Users")
            .Accepts<UpdateUserRequest>("application/json")
            .Produces<UserDetailDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapPut("/{userId:guid}/password", UpdateUserPasswordAsync)
            .WithName("Users_Security_UpdatePassword")
            .WithSummary("Update user password (Admin)")
            .WithDescription("Changes the specified user's password by validating the current password first. Requires Admin role.")
            .WithTags("User Security")
            .Accepts<UpdateUserPasswordRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        return group;
    }

    private static async Task<IResult> UpdateUserAsync(
        string userId,
        UpdateUserRequest body,
        ISender mediator)
    {
        var result = await mediator.Send(new UpdateUserCommand(userId, body.DisplayName, body.Email));
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> UpdateUserPasswordAsync(
        string userId,
        UpdateUserPasswordRequest body,
        ISender mediator)
    {
        await mediator.Send(new UpdateUserPasswordCommand(userId, body.CurrentPassword, body.NewPassword));
        return TypedResults.NoContent();
    }

    public sealed record UpdateUserRequest(string DisplayName, string Email);

    public sealed record UpdateUserPasswordRequest(string CurrentPassword, string NewPassword);
}
