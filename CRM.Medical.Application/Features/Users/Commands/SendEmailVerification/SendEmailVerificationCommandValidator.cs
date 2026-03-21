using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.SendEmailVerification;

public sealed class SendEmailVerificationCommandValidator : AbstractValidator<SendEmailVerificationCommand>
{
    public SendEmailVerificationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ConfirmationBaseUrl).NotEmpty().Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute));
    }
}
