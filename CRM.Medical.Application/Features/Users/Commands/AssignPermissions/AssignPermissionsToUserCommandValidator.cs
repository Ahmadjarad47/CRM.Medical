using CRM.Medical.Application.Features.Users.Constants;
using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.AssignPermissions;

public sealed class AssignPermissionsToUserCommandValidator
    : AbstractValidator<AssignPermissionsToUserCommand>
{
    public AssignPermissionsToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Permissions)
            .NotEmpty().WithMessage("At least one permission must be specified.");

        RuleForEach(x => x.Permissions)
            .NotEmpty().WithMessage("Permission name cannot be empty.")
            .Must(p => UserPermissions.All.Contains(p))
            .WithMessage((_, p) => $"'{p}' is not a recognised permission. Valid values: {string.Join(", ", UserPermissions.All)}.");
    }
}
