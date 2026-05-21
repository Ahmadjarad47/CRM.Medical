using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(
    UserManager<User> userManager,
    ICurrentUserAccessor currentUser,
    IAccessPolicyEvaluator accessPolicyEvaluator,
    IAccessPolicyReadService accessPolicyReadService)
    : IRequestHandler<GetUsersQuery, PagedResult<UserSummaryDto>>
{
    public async Task<PagedResult<UserSummaryDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        _ = currentUser.GetRequiredUserId();

        var query = userManager.Users.AsNoTracking();
        query = await accessPolicyEvaluator.ApplyAsync(query, "users", "read", cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLowerInvariant();
            query = query.Where(u =>
                u.FullName.ToLower().Contains(term) ||
                u.Email!.ToLower().Contains(term));
        }

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        if (!string.IsNullOrEmpty(request.Role))
        {
            var roleUsers = await userManager.GetUsersInRoleAsync(request.Role);
            var roleUserIds = roleUsers.Select(u => u.Id).ToHashSet();
            query = query.Where(u => roleUserIds.Contains(u.Id));
        }

        query = (request.SortBy.ToLowerInvariant(), request.SortDescending) switch
        {
            ("email", false)      => query.OrderBy(u => u.Email),
            ("email", true)       => query.OrderByDescending(u => u.Email),
            ("createdat", false)  => query.OrderBy(u => u.CreatedAt),
            ("createdat", true)   => query.OrderByDescending(u => u.CreatedAt),
            ("isactive", false)   => query.OrderBy(u => u.IsActive),
            ("isactive", true)    => query.OrderByDescending(u => u.IsActive),
            (_, false)            => query.OrderBy(u => u.FullName),
            (_, true)             => query.OrderByDescending(u => u.FullName),
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userRoleMap = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userRoleMap[user.Id] = roles.ToList().AsReadOnly();
        }

        var roleNames = userRoleMap.Values.SelectMany(x => x).Distinct(StringComparer.OrdinalIgnoreCase);
        var accessPoliciesByRole = await accessPolicyReadService.GetPoliciesForRolesAsync(roleNames, cancellationToken);

        var items = users
            .Select(user =>
            {
                userRoleMap.TryGetValue(user.Id, out var roles);
                roles ??= [];

                var accessPolicies = roles
                    .SelectMany(roleName => accessPoliciesByRole.TryGetValue(roleName, out var policies) ? policies : [])
                    .DistinctBy(policy => policy.Id)
                    .ToList();

                return new UserSummaryDto(
                    user.Id,
                    user.Email!,
                    user.FullName,
                    user.City,
                    user.PhoneNumber,
                    user.IsActive,
                    user.EmailConfirmed,
                    user.CreatedAt,
                    user.CreatedByUserId,
                    roles,
                    accessPolicies);
            })
            .ToList();

        var result = new PagedResult<UserSummaryDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return result;
    }
}
