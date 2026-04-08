using CRM.Medical.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommandHandler(RoleManager<IdentityRole> roleManager)
    : IRequestHandler<UpdateRoleCommand>
{
    public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id)
            ?? throw new ApplicationNotFoundException($"Role '{request.Id}' was not found.");

        var result = await roleManager.SetRoleNameAsync(role, request.Name);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
