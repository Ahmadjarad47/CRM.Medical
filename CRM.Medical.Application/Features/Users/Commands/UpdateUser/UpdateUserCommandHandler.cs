using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(UserManager<User> userManager)
    : IRequestHandler<UpdateUserCommand, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        user.DisplayName = request.DisplayName.Trim();
        user.UserType = request.UserType;
        user.DateOfBirth = request.DateOfBirth;
        user.PhoneSecondary = NormalizeOptional(request.PhoneSecondary);
        user.AddressLine1 = NormalizeOptional(request.AddressLine1);
        user.AddressLine2 = NormalizeOptional(request.AddressLine2);
        user.City = NormalizeOptional(request.City);
        user.Region = NormalizeOptional(request.Region);
        user.PostalCode = NormalizeOptional(request.PostalCode);
        user.Country = NormalizeOptional(request.Country);
        user.NationalIdNumber = NormalizeOptional(request.NationalIdNumber);
        user.InsuranceProvider = NormalizeOptional(request.InsuranceProvider);
        user.InsurancePolicyNumber = NormalizeOptional(request.InsurancePolicyNumber);
        user.EmergencyContactName = NormalizeOptional(request.EmergencyContactName);
        user.EmergencyContactPhone = NormalizeOptional(request.EmergencyContactPhone);
        user.MedicalLicenseNumber = NormalizeOptional(request.MedicalLicenseNumber);
        user.Specialty = NormalizeOptional(request.Specialty);
        user.ClinicName = NormalizeOptional(request.ClinicName);
        user.LabName = NormalizeOptional(request.LabName);
        user.LabLicenseNumber = NormalizeOptional(request.LabLicenseNumber);
        user.LabContactName = NormalizeOptional(request.LabContactName);
        user.LabContactPhone = NormalizeOptional(request.LabContactPhone);
        user.SpecialAccountCode = NormalizeOptional(request.SpecialAccountCode);
        user.SpecialNotes = NormalizeOptional(request.SpecialNotes);

        var normalizedEmail = request.Email.Trim();
        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
        {
            var setEmailResult = await userManager.SetEmailAsync(user, normalizedEmail);
            setEmailResult.ThrowIfFailed(nameof(request.Email));

            var setUserNameResult = await userManager.SetUserNameAsync(user, normalizedEmail);
            setUserNameResult.ThrowIfFailed(nameof(request.Email));
        }

        var updateResult = await userManager.UpdateAsync(user);
        updateResult.ThrowIfFailed(nameof(UpdateUserCommand));

        return user.ToDetailDto();
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }
}
