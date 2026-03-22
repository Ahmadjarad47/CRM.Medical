using FluentValidation;

namespace CRM.Medical.Application.Features.Auth.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
        RuleFor(x => x.Token).NotEmpty().MaximumLength(4096);
        RuleFor(x => x.NewPassword).NotEmpty().MaximumLength(256);
    }
}
