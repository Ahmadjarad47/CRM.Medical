using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.AssignRoles;

public sealed class AssignRolesCommandValidator : AbstractValidator<AssignRolesCommand>
{
    public AssignRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Roles).NotEmpty().WithMessage("At least one role must be specified.");
        RuleForEach(x => x.Roles).NotEmpty().WithMessage("Role name cannot be empty.");
    }
}
