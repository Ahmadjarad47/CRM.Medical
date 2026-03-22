using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.AssignRoles;

public sealed class AssignRolesCommandHandler(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager) : IRequestHandler<AssignRolesCommand, UserRolesDto>
{
    public async Task<UserRolesDto> Handle(AssignRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var normalizedRoles = request.Roles
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var role in normalizedRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                throw new KeyNotFoundException($"Role '{role}' does not exist.");
        }

        var result = await userManager.AddToRolesAsync(user, normalizedRoles);
        result.ThrowIfFailed(nameof(AssignRolesCommand));

        var roles = await userManager.GetRolesAsync(user);
        return user.ToRolesDto(roles);
    }
}
