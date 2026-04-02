using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;

public sealed class GetUserPermissionsQueryHandler(UserManager<User> userManager)
    : IRequestHandler<GetUserPermissionsQuery, UserPermissionsDto>
{
    public async Task<UserPermissionsDto> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

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
