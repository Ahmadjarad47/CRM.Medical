using CRM.Medical.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Roles.Commands.DeleteRole;

public sealed class DeleteRoleCommandHandler(RoleManager<IdentityRole> roleManager)
    : IRequestHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id)
            ?? throw new ApplicationNotFoundException($"Role '{request.Id}' was not found.");

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
