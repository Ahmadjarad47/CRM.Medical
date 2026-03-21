using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Application.Features.Users.Queries.GetAllRoles;

public sealed class GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager)
    : IRequestHandler<GetAllRolesQuery, IReadOnlyList<string>>
{
    public async Task<IReadOnlyList<string>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles
            .AsNoTracking()
            .Select(x => x.Name)
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x))
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        return roles!;
    }
}
