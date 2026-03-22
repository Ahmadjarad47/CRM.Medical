using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(UserManager<User> userManager)
    : IRequestHandler<CreateUserCommand, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim();

        var user = new User
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            DisplayName = request.DisplayName.Trim(),
            UserType = request.UserType,
            DateOfBirth = request.DateOfBirth,
            PhoneSecondary = NormalizeOptional(request.PhoneSecondary),
            AddressLine1 = NormalizeOptional(request.AddressLine1),
            AddressLine2 = NormalizeOptional(request.AddressLine2),
            City = NormalizeOptional(request.City),
            Region = NormalizeOptional(request.Region),
            PostalCode = NormalizeOptional(request.PostalCode),
            Country = NormalizeOptional(request.Country),
            NationalIdNumber = NormalizeOptional(request.NationalIdNumber),
            InsuranceProvider = NormalizeOptional(request.InsuranceProvider),
            InsurancePolicyNumber = NormalizeOptional(request.InsurancePolicyNumber),
            EmergencyContactName = NormalizeOptional(request.EmergencyContactName),
            EmergencyContactPhone = NormalizeOptional(request.EmergencyContactPhone),
            MedicalLicenseNumber = NormalizeOptional(request.MedicalLicenseNumber),
            Specialty = NormalizeOptional(request.Specialty),
            ClinicName = NormalizeOptional(request.ClinicName),
            LabName = NormalizeOptional(request.LabName),
            LabLicenseNumber = NormalizeOptional(request.LabLicenseNumber),
            LabContactName = NormalizeOptional(request.LabContactName),
            LabContactPhone = NormalizeOptional(request.LabContactPhone),
            SpecialAccountCode = NormalizeOptional(request.SpecialAccountCode),
            SpecialNotes = NormalizeOptional(request.SpecialNotes),
        };

        var result = await userManager.CreateAsync(user, request.Password);
        result.ThrowIfFailed(nameof(CreateUserCommand));

        return user.ToDetailDto();
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }
}
