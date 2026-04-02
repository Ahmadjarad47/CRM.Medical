using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.AssignRoles;

public sealed class AssignRolesCommandHandler(
    UserManager<User> userManager,
    ICacheService cache)
    : IRequestHandler<AssignRolesCommand>
{
    public async Task Handle(AssignRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToAdd = request.Roles
            .Except(currentRoles, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (rolesToAdd.Count == 0)
            return;

        var result = await userManager.AddToRolesAsync(user, rolesToAdd);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
