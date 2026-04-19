using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(
    UserManager<User> userManager,
    IDateTimeProvider dateTimeProvider,
    ICacheService cache,
    IUserManagementAccess userManagementAccess,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<UpdateUserCommand, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var actorId = currentUser.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        await userManagementAccess.EnsureActorCanManageUserAsync(actorId, user, cancellationToken);

        user.FullName = request.FullName;
        user.City = request.City;
        user.PhoneNumber = request.PhoneNumber;
        user.UpdatedAt = dateTimeProvider.UtcNow;

        if (request.ProfileMetadata is not null)
        {
            user.ProfileMetadata?.Dispose();
            user.ProfileMetadata = ProfileMetadataMapper.ToJsonDocument(request.ProfileMetadata);
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);

        var roles = await userManager.GetRolesAsync(user);

        return new UserDetailDto(
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
            ProfileMetadataMapper.ToJsonElement(user.ProfileMetadata));
    }
}
