using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.LockUser;

public sealed class LockUserCommandValidator : AbstractValidator<LockUserCommand>
{
    public LockUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LockoutMinutes).GreaterThan(0);
    }
}
