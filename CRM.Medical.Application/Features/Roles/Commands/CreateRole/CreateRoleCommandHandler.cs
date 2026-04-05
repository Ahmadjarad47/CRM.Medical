using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Roles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler(RoleManager<IdentityRole> roleManager)
    : IRequestHandler<CreateRoleCommand, RoleDto>
{
    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new IdentityRole(request.Name);
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));

        return new RoleDto(role.Id, role.Name!);
    }
}
