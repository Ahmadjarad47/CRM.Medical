using CRM.Medical.Application.Features.Users.Commands.AddUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.RemoveUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.UpdateUserPermissions;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Queries.GetAllPermissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Endpoints.Users;

public static class UserPermissionEndpoints
{
    private const string AdminRole = "Admin";

    public static RouteGroupBuilder MapUserPermissionEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/permissions", GetAllPermissionsAsync)
            .WithName("Users_Permissions_GetAll")
            .WithSummary("List available permissions (Admin)")
            .WithDescription("Returns all distinct permission claims known by the system. Requires Admin role.")
            .WithTags("User Permissions")
            .Produces<IReadOnlyList<string>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapPost("/{userId:guid}/permissions", AddPermissionsAsync)
            .WithName("Users_Permissions_Add")
            .WithSummary("Add permissions to a user (Admin)")
            .WithDescription("Adds permission claims to the specified user without removing existing ones. Requires Admin role.")
            .WithTags("User Permissions")
            .Accepts<ManagePermissionsRequest>("application/json")
            .Produces<UserPermissionsDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapPut("/{userId:guid}/permissions", UpdatePermissionsAsync)
            .WithName("Users_Permissions_Replace")
            .WithSummary("Replace user permissions (Admin)")
            .WithDescription("Replaces the current set of user permission claims with the provided set. Requires Admin role.")
            .WithTags("User Permissions")
            .Accepts<ManagePermissionsRequest>("application/json")
            .Produces<UserPermissionsDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapDelete("/{userId:guid}/permissions", RemovePermissionsAsync)
            .WithName("Users_Permissions_Remove")
            .WithSummary("Remove permissions from a user (Admin)")
            .WithDescription("Removes specific permission claims from the specified user. Requires Admin role.")
            .WithTags("User Permissions")
            .Accepts<ManagePermissionsRequest>("application/json")
            .Produces<UserPermissionsDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        return group;
    }

    private static async Task<IResult> GetAllPermissionsAsync(ISender mediator)
    {
        var result = await mediator.Send(new GetAllPermissionsQuery());
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> AddPermissionsAsync(string userId, [FromBody] ManagePermissionsRequest body, ISender mediator)
    {
        var result = await mediator.Send(new AddUserPermissionsCommand(userId, body.Permissions));
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> UpdatePermissionsAsync(string userId, [FromBody] ManagePermissionsRequest body, ISender mediator)
    {
        var result = await mediator.Send(new UpdateUserPermissionsCommand(userId, body.Permissions));
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> RemovePermissionsAsync(string userId, [FromBody] ManagePermissionsRequest body, ISender mediator)
    {
        var result = await mediator.Send(new RemoveUserPermissionsCommand(userId, body.Permissions));
        return TypedResults.Ok(result);
    }

    public sealed record ManagePermissionsRequest(IReadOnlyCollection<string> Permissions);
}
