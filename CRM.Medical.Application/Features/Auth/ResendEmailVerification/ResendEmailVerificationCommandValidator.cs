using FluentValidation;

namespace CRM.Medical.Application.Features.Auth.ResendEmailVerification;

public sealed class ResendEmailVerificationCommandValidator
    : AbstractValidator<ResendEmailVerificationCommand>
{
    public ResendEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
