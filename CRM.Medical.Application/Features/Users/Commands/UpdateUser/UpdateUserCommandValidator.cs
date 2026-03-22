using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
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
    }
}
