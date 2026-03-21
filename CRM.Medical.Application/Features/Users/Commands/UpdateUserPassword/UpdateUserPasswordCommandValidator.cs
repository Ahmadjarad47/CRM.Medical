using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUserPassword;

public sealed class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrentPassword).NotEmpty().MaximumLength(512);
        RuleFor(x => x.NewPassword).NotEmpty().MaximumLength(512);
    }
}
