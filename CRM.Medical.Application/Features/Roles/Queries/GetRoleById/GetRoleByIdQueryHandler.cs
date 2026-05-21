using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Roles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler(
    RoleManager<IdentityRole> roleManager,
    IAccessPolicyReadService accessPolicyReadService)
    : IRequestHandler<GetRoleByIdQuery, RoleDto>
{
    public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id)
            ?? throw new ApplicationNotFoundException($"Role '{request.Id}' was not found.");

        var accessPoliciesByRole = await accessPolicyReadService.GetPoliciesForRolesAsync([role.Name!], cancellationToken);
        return new RoleDto(
            role.Id,
            role.Name!,
            accessPoliciesByRole.TryGetValue(role.Name!, out var policies) ? policies : []);
    }
}
