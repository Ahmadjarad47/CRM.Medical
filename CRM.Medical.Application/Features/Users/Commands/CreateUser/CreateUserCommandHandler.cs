using System.Text.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    UserManager<User> userManager,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateUserCommand, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new ApplicationConflictException($"A user with email '{request.Email}' already exists.");

        var now = dateTimeProvider.UtcNow;
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            City = request.City,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            EmailConfirmed = true, // admin-created users bypass email verification
            CreatedAt = now,
            ProfileMetadata = request.ProfileMetadata.HasValue
                ? JsonDocument.Parse(request.ProfileMetadata.Value.GetRawText())
                : null
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", createResult.Errors.Select(e => e.Description)));

        if (request.Roles.Count > 0)
        {
            var rolesResult = await userManager.AddToRolesAsync(user, request.Roles);
            if (!rolesResult.Succeeded)
                throw new ApplicationBadRequestException(
                    string.Join("; ", rolesResult.Errors.Select(e => e.Description)));
        }

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
            roles.ToList().AsReadOnly(),
            request.ProfileMetadata);
    }
}
