using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler(
    UserManager<User> userManager,
    IDateTimeProvider dateTimeProvider,
    IEmailVerificationSender emailSender)
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new ApplicationConflictException(
                $"An account with email '{request.Email}' already exists.");

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            City = request.City,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            EmailConfirmed = false, // requires email verification
            CreatedAt = dateTimeProvider.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", createResult.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, request.Role);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await emailSender.SendAsync(user.Email!, token, cancellationToken);

        return new RegisterResponse(
            "Registration successful. Please check your email to verify your account.");
    }
}
