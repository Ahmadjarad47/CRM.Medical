using CRM.Medical.Application.Features.Users.Commands.AssignRoles;
using CRM.Medical.Application.Features.Users.Commands.RemoveRoles;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Endpoints.Users;

public static class UserRoleEndpoints
{
    private const string AdminRole = "Admin";

    public static RouteGroupBuilder MapUserRoleEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/roles", GetAllRolesAsync)
            .WithName("Users_Roles_GetAll")
            .WithSummary("List available roles (Admin)")
            .WithDescription("Returns all role names available for assignment. Requires Admin role.")
            .WithTags("User Roles")
            .Produces<IReadOnlyList<string>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapPost("/{userId:guid}/roles", AssignRolesAsync)
            .WithName("Users_Roles_Assign")
            .WithSummary("Assign roles to a user (Admin)")
            .WithDescription("Assigns one or more roles to the specified user. Requires Admin role.")
            .WithTags("User Roles")
            .Accepts<ManageRolesRequest>("application/json")
            .Produces<UserRolesDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapDelete("/{userId:guid}/roles", RemoveRolesAsync)
            .WithName("Users_Roles_Remove")
            .WithSummary("Remove roles from a user (Admin)")
            .WithDescription("Removes one or more roles from the specified user. Requires Admin role.")
            .WithTags("User Roles")
            .Accepts<ManageRolesRequest>("application/json")
            .Produces<UserRolesDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        return group;
    }

    private static async Task<IResult> GetAllRolesAsync(ISender mediator)
    {
        var result = await mediator.Send(new GetAllRolesQuery());
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> AssignRolesAsync(string userId, [FromBody] ManageRolesRequest body, ISender mediator)
    {
        var result = await mediator.Send(new AssignRolesCommand(userId, body.Roles));
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> RemoveRolesAsync(string userId, [FromBody] ManageRolesRequest body, ISender mediator)
    {
        var result = await mediator.Send(new RemoveRolesCommand(userId, body.Roles));
        return TypedResults.Ok(result);
    }

    public sealed record ManageRolesRequest(IReadOnlyCollection<string> Roles);
}
