using FluentValidation;

namespace CRM.Medical.Application.Features.Auth.ResendEmailVerification;

public sealed class ResendEmailVerificationCommandValidator : AbstractValidator<ResendEmailVerificationCommand>
{
    public ResendEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
        RuleFor(x => x.ConfirmationBaseUrl).NotEmpty().MaximumLength(2048);
    }
}
