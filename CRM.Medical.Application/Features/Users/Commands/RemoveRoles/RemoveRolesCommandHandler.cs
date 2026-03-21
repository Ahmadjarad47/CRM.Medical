using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.RemoveRoles;

public sealed class RemoveRolesCommandHandler(UserManager<User> userManager)
    : IRequestHandler<RemoveRolesCommand, UserRolesDto>
{
    public async Task<UserRolesDto> Handle(RemoveRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var normalizedRoles = request.Roles
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var result = await userManager.RemoveFromRolesAsync(user, normalizedRoles);
        result.ThrowIfFailed(nameof(RemoveRolesCommand));

        var roles = await userManager.GetRolesAsync(user);
        return new UserRolesDto(user.Id, roles.OrderBy(x => x).ToArray());
    }
}
