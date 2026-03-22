using CRM.Medical.Domain.Entities;
using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MaximumLength(256);
        RuleFor(x => x.UserType).IsInEnum();
        RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.PhoneSecondary).MaximumLength(32);
        RuleFor(x => x.AddressLine1).MaximumLength(256);
        RuleFor(x => x.AddressLine2).MaximumLength(256);
        RuleFor(x => x.City).MaximumLength(128);
        RuleFor(x => x.Region).MaximumLength(128);
        RuleFor(x => x.PostalCode).MaximumLength(32);
        RuleFor(x => x.Country).MaximumLength(128);

        RuleFor(x => x.NationalIdNumber).MaximumLength(128);
        RuleFor(x => x.InsuranceProvider).MaximumLength(256);
        RuleFor(x => x.InsurancePolicyNumber).MaximumLength(128);
        RuleFor(x => x.EmergencyContactName).MaximumLength(256);
        RuleFor(x => x.EmergencyContactPhone).MaximumLength(32);

        RuleFor(x => x.MedicalLicenseNumber).MaximumLength(128);
        RuleFor(x => x.Specialty).MaximumLength(256);
        RuleFor(x => x.ClinicName).MaximumLength(256);

        RuleFor(x => x.LabName).MaximumLength(256);
        RuleFor(x => x.LabLicenseNumber).MaximumLength(128);
        RuleFor(x => x.LabContactName).MaximumLength(256);
        RuleFor(x => x.LabContactPhone).MaximumLength(32);

        RuleFor(x => x.SpecialAccountCode).MaximumLength(128);
        RuleFor(x => x.SpecialNotes).MaximumLength(2048);

        RuleFor(x => x).Custom((request, context) =>
        {
            switch (request.UserType)
            {
                case UserType.Patient:
                    RequireDateOfBirth(request, context);
                    EnsureDoctorFieldsAreEmpty(request, context);
                    EnsureLabFieldsAreEmpty(request, context);
                    EnsureSpecialFieldsAreEmpty(request, context);
                    break;
                case UserType.Doctor:
                    RequireNonEmpty(request.MedicalLicenseNumber, nameof(request.MedicalLicenseNumber), "Medical license number is required for doctor users.", context);
                    EnsurePatientFieldsAreEmpty(request, context);
                    EnsureLabFieldsAreEmpty(request, context);
                    EnsureSpecialFieldsAreEmpty(request, context);
                    break;
                case UserType.Lab:
                    RequireNonEmpty(request.LabName, nameof(request.LabName), "Lab name is required for lab users.", context);
                    RequireNonEmpty(request.LabLicenseNumber, nameof(request.LabLicenseNumber), "Lab license number is required for lab users.", context);
                    EnsurePatientFieldsAreEmpty(request, context);
                    EnsureDoctorFieldsAreEmpty(request, context);
                    EnsureSpecialFieldsAreEmpty(request, context);
                    break;
                case UserType.Special:
                    RequireNonEmpty(request.SpecialAccountCode, nameof(request.SpecialAccountCode), "Special account code is required for special users.", context);
                    EnsurePatientFieldsAreEmpty(request, context);
                    EnsureDoctorFieldsAreEmpty(request, context);
                    EnsureLabFieldsAreEmpty(request, context);
                    break;
                case UserType.Admin:
                    EnsurePatientFieldsAreEmpty(request, context);
                    EnsureDoctorFieldsAreEmpty(request, context);
                    EnsureLabFieldsAreEmpty(request, context);
                    EnsureSpecialFieldsAreEmpty(request, context);
                    break;
            }
        });
    }

    private static void RequireDateOfBirth(CreateUserCommand request, ValidationContext<CreateUserCommand> context)
    {
        if (request.DateOfBirth is null)
        {
            context.AddFailure(nameof(request.DateOfBirth), "Date of birth is required for patient users.");
        }
    }

    private static void EnsurePatientFieldsAreEmpty(CreateUserCommand request, ValidationContext<CreateUserCommand> context)
    {
        EnsureEmpty(request.NationalIdNumber, nameof(request.NationalIdNumber), context);
        EnsureEmpty(request.InsuranceProvider, nameof(request.InsuranceProvider), context);
        EnsureEmpty(request.InsurancePolicyNumber, nameof(request.InsurancePolicyNumber), context);
        EnsureEmpty(request.EmergencyContactName, nameof(request.EmergencyContactName), context);
        EnsureEmpty(request.EmergencyContactPhone, nameof(request.EmergencyContactPhone), context);
    }

    private static void EnsureDoctorFieldsAreEmpty(CreateUserCommand request, ValidationContext<CreateUserCommand> context)
    {
        EnsureEmpty(request.MedicalLicenseNumber, nameof(request.MedicalLicenseNumber), context);
        EnsureEmpty(request.Specialty, nameof(request.Specialty), context);
        EnsureEmpty(request.ClinicName, nameof(request.ClinicName), context);
    }

    private static void EnsureLabFieldsAreEmpty(CreateUserCommand request, ValidationContext<CreateUserCommand> context)
    {
        EnsureEmpty(request.LabName, nameof(request.LabName), context);
        EnsureEmpty(request.LabLicenseNumber, nameof(request.LabLicenseNumber), context);
        EnsureEmpty(request.LabContactName, nameof(request.LabContactName), context);
        EnsureEmpty(request.LabContactPhone, nameof(request.LabContactPhone), context);
    }

    private static void EnsureSpecialFieldsAreEmpty(CreateUserCommand request, ValidationContext<CreateUserCommand> context)
    {
        EnsureEmpty(request.SpecialAccountCode, nameof(request.SpecialAccountCode), context);
        EnsureEmpty(request.SpecialNotes, nameof(request.SpecialNotes), context);
    }

    private static void EnsureEmpty(string? value, string propertyName, ValidationContext<CreateUserCommand> context)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            context.AddFailure(propertyName, $"'{propertyName}' is not allowed for selected user type.");
        }
    }

    private static void RequireNonEmpty(
        string? value,
        string propertyName,
        string errorMessage,
        ValidationContext<CreateUserCommand> context)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            context.AddFailure(propertyName, errorMessage);
        }
    }
}
