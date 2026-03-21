using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.RemoveRoles;

public sealed class RemoveRolesCommandValidator : AbstractValidator<RemoveRolesCommand>
{
    public RemoveRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Roles).NotNull().NotEmpty();
        RuleForEach(x => x.Roles).NotEmpty().MaximumLength(256);
    }
}
