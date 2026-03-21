using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.AssignRoles;

public sealed class AssignRolesCommandValidator : AbstractValidator<AssignRolesCommand>
{
    public AssignRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Roles).NotNull().NotEmpty();
        RuleForEach(x => x.Roles).NotEmpty().MaximumLength(256);
    }
}
