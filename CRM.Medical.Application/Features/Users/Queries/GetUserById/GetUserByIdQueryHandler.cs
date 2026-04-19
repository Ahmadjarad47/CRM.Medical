using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(
    UserManager<User> userManager,
    ICacheService cache,
    IUserManagementAccess userManagementAccess,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<GetUserByIdQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var actorId = currentUser.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        await userManagementAccess.EnsureActorCanManageUserAsync(actorId, user, cancellationToken);

        var actor = await userManager.FindByIdAsync(actorId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        var isAdmin = await userManager.IsInRoleAsync(actor, UserRoles.Admin);

        if (isAdmin)
        {
            var cacheKey = CacheKeys.UserById(request.UserId);
            var cached = await cache.GetAsync<UserDetailDto>(cacheKey, cancellationToken);
            if (cached is not null)
                return cached;
        }

        var roles = await userManager.GetRolesAsync(user);

        var dto = new UserDetailDto(
            user.Id,
            user.Email!,
            user.FullName,
            user.City,
            user.PhoneNumber,
            user.IsActive,
            user.EmailConfirmed,
            user.LockoutEnabled,
            user.LockoutEnd,
            user.CreatedAt,
            user.UpdatedAt,
            user.CreatedByUserId,
            roles.ToList().AsReadOnly(),
            user.ProfileMetadata?.RootElement);

        if (isAdmin)
            await cache.SetAsync(CacheKeys.UserById(request.UserId), dto, CacheKeys.UserDetailExpiry, cancellationToken);

        return dto;
    }
}
