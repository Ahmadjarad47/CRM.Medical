using System.Linq.Expressions;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Roles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Application.Features.Roles.Queries.GetRoles;

public sealed class GetRolesQueryHandler(
    RoleManager<IdentityRole> roleManager,
    IAccessPolicyReadService accessPolicyReadService)
    : IRequestHandler<GetRolesQuery, PagedResult<RoleDto>>
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<IdentityRole, string?>>> SearchFields =
        new Dictionary<string, Expression<Func<IdentityRole, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = r => r.Name
        };

    public async Task<PagedResult<RoleDto>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(request.Page, request.PageSize);
        var query = roleManager.Roles
            .AsNoTracking()
            .ApplyAdvancedSearch(request.Search, SearchFields, r => r.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var roles = await query
            .OrderBy(r => r.Name!)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        var roleNames = roles
            .Select(role => role.Name)
            .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
            .Cast<string>()
            .ToArray();
        var accessPoliciesByRole = await accessPolicyReadService.GetPoliciesForRolesAsync(roleNames, cancellationToken);

        var items = roles
            .Select(role => new RoleDto(
                role.Id,
                role.Name!,
                accessPoliciesByRole.TryGetValue(role.Name!, out var policies) ? policies : []))
            .ToList();

        return new PagedResult<RoleDto>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }
}
