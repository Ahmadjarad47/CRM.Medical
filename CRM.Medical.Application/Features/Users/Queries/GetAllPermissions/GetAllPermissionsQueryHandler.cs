using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Application.Features.Users.Queries.GetAllPermissions;

public sealed class GetAllPermissionsQueryHandler(UserManager<User> userManager)
    : IRequestHandler<GetAllPermissionsQuery, IReadOnlyList<string>>
{
    public async Task<IReadOnlyList<string>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var user in users)
        {
            var claims = await userManager.GetClaimsAsync(user);
            foreach (var claim in claims.Where(x => x.Type == UserPermissionClaimTypes.Permission))
            {
                if (!string.IsNullOrWhiteSpace(claim.Value))
                    permissions.Add(claim.Value.Trim());
            }
        }

        return permissions.OrderBy(x => x).ToArray();
    }
}
