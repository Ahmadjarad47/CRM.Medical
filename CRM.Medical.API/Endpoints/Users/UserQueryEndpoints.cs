using CRM.Medical.Application.Features.Users.Queries.GetUserById;
using CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;
using CRM.Medical.Application.Features.Users.Queries.GetUserRoles;
using CRM.Medical.Application.Features.Users.Queries.GetUsers;
using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.API.Endpoints.Users;

public static class UserQueryEndpoints
{
    private const string AdminRole = "Admin";

    public static RouteGroupBuilder MapUserQueryEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetUsersAsync)
            .WithName("Users_GetAll")
            .WithSummary("List all users (Admin)")
            .WithDescription("Returns summary profile information for all users. Requires Admin role.")
            .WithTags("Users")
            .Produces<IReadOnlyList<UserSummaryDto>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapGet("/{userId:guid}", GetUserByIdAsync)
            .WithName("Users_GetById")
            .WithSummary("Get user details by ID (Admin)")
            .WithDescription("Returns detailed profile information for a specific user. Requires Admin role.")
            .WithTags("Users")
            .Produces<UserDetailDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapGet("/{userId:guid}/roles", GetUserRolesAsync)
            .WithName("Users_Roles_GetByUserId")
            .WithSummary("Get assigned roles for a user (Admin)")
            .WithDescription("Returns all roles currently assigned to the specified user. Requires Admin role.")
            .WithTags("User Roles")
            .Produces<UserRolesDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapGet("/{userId:guid}/permissions", GetUserPermissionsAsync)
            .WithName("Users_Permissions_GetByUserId")
            .WithSummary("Get assigned permissions for a user (Admin)")
            .WithDescription("Returns all explicit permission claims assigned to the specified user. Requires Admin role.")
            .WithTags("User Permissions")
            .Produces<UserPermissionsDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        return group;
    }

    private static async Task<IResult> GetUsersAsync(ISender mediator)
    {
        var result = await mediator.Send(new GetUsersQuery());
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetUserByIdAsync(string userId, ISender mediator)
    {
        var result = await mediator.Send(new GetUserByIdQuery(userId));
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetUserRolesAsync(string userId, ISender mediator)
    {
        var result = await mediator.Send(new GetUserRolesQuery(userId));
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetUserPermissionsAsync(string userId, ISender mediator)
    {
        var result = await mediator.Send(new GetUserPermissionsQuery(userId));
        return TypedResults.Ok(result);
    }
}
