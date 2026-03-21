using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserRoles;

public sealed class GetUserRolesQueryHandler(UserManager<User> userManager)
    : IRequestHandler<GetUserRolesQuery, UserRolesDto>
{
    public async Task<UserRolesDto> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var roles = await userManager.GetRolesAsync(user);
        return new UserRolesDto(user.Id, roles.OrderBy(x => x).ToArray());
    }
}
