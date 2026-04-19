using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;

public sealed class GetUserPermissionsQueryHandler(
    UserManager<User> userManager,
    IUserManagementAccess userManagementAccess,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<GetUserPermissionsQuery, UserPermissionsDto>
{
    public async Task<UserPermissionsDto> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var actorId = currentUser.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        await userManagementAccess.EnsureActorCanManageUserAsync(actorId, user, cancellationToken);

        var claims = await userManager.GetClaimsAsync(user);

        var permissions = claims
            .Where(c => c.Type == UserPermissions.ClaimType)
            .Select(c => c.Value)
            .OrderBy(p => p)
            .ToList()
            .AsReadOnly();

        return new UserPermissionsDto(user.Id, permissions);
    }
}
