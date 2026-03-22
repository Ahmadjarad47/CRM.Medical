using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(UserManager<User> userManager)
    : IRequestHandler<UpdateUserCommand, UserDetailDto>
{
    private const string PatientRole = "Patient";
    private const string DoctorRole = "Doctor";
    private const string LabRole = "Lab";
    private const string SpecialRole = "Special";

    public async Task<UserDetailDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var roles = await userManager.GetRolesAsync(user);
        EnsureRoleRequiredFields(request, roles);

        user.DisplayName = request.DisplayName.Trim();
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

    private static void EnsureRoleRequiredFields(UpdateUserCommand request, IEnumerable<string> roles)
    {
        var normalizedRoles = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var failures = new List<ValidationFailure>();

        if (normalizedRoles.Contains(PatientRole) && request.DateOfBirth is null)
        {
            failures.Add(new ValidationFailure(
                nameof(request.DateOfBirth),
                "Date of birth is required for users assigned to the Patient role."));
        }

        if (normalizedRoles.Contains(DoctorRole) && string.IsNullOrWhiteSpace(request.MedicalLicenseNumber))
        {
            failures.Add(new ValidationFailure(
                nameof(request.MedicalLicenseNumber),
                "Medical license number is required for users assigned to the Doctor role."));
        }

        if (normalizedRoles.Contains(LabRole))
        {
            if (string.IsNullOrWhiteSpace(request.LabName))
            {
                failures.Add(new ValidationFailure(
                    nameof(request.LabName),
                    "Lab name is required for users assigned to the Lab role."));
            }

            if (string.IsNullOrWhiteSpace(request.LabLicenseNumber))
            {
                failures.Add(new ValidationFailure(
                    nameof(request.LabLicenseNumber),
                    "Lab license number is required for users assigned to the Lab role."));
            }
        }

        if (normalizedRoles.Contains(SpecialRole) && string.IsNullOrWhiteSpace(request.SpecialAccountCode))
        {
            failures.Add(new ValidationFailure(
                nameof(request.SpecialAccountCode),
                "Special account code is required for users assigned to the Special role."));
        }

        if (failures.Count > 0)
            throw new ValidationException(failures);
    }
}
