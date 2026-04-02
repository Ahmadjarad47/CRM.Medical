using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(
    UserManager<User> userManager,
    ICacheService cache)
    : IRequestHandler<GetUsersQuery, PagedResult<UserSummaryDto>>
{
    public async Task<PagedResult<UserSummaryDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.UserList(
            request.Page, request.PageSize,
            request.SearchTerm, request.IsActive, request.Role);

        var cached = await cache.GetAsync<PagedResult<UserSummaryDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var query = userManager.Users.AsNoTracking();

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
            .Select(u => new UserSummaryDto(
                u.Id,
                u.Email!,
                u.FullName,
                u.City,
                u.PhoneNumber,
                u.IsActive,
                u.EmailConfirmed,
                u.CreatedAt))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<UserSummaryDto>
        {
            Items = users,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        await cache.SetAsync(cacheKey, result, CacheKeys.UserListExpiry, cancellationToken);
        return result;
    }
}
