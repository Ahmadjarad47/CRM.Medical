using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(
    UserManager<User> userManager,
    ICacheService cache)
    : IRequestHandler<GetUserByIdQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.UserById(request.UserId);

        var cached = await cache.GetAsync<UserDetailDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

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
            roles.ToList().AsReadOnly(),
            user.ProfileMetadata?.RootElement);

        await cache.SetAsync(cacheKey, dto, CacheKeys.UserDetailExpiry, cancellationToken);
        return dto;
    }
}
