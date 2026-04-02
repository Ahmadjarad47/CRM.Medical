using CRM.Medical.Application.Features.Users.Constants;
using FluentValidation;

namespace CRM.Medical.Application.Features.Auth.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    // Roles a user can self-select — Admin is excluded (granted only by admins)
    private static readonly IReadOnlySet<string> SelfRegistrableRoles =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            UserRoles.Doctor,
            UserRoles.Patient,
            UserRoles.LabPartner,
            UserRoles.User
        };

    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => x.City is not null);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .When(x => x.PhoneNumber is not null);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => SelfRegistrableRoles.Contains(r))
            .WithMessage(
                $"Role must be one of: {string.Join(", ", SelfRegistrableRoles)}. " +
                $"'{UserRoles.Admin}' can only be assigned by an administrator.");
    }
}
