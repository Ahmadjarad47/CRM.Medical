using CRM.Medical.Application.Abstractions;
using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.AssignPermissions;

public sealed class AssignPermissionsToUserCommandValidator
    : AbstractValidator<AssignPermissionsToUserCommand>
{
    public AssignPermissionsToUserCommandValidator(IPermissionRepository permissions)
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Permissions)
            .NotEmpty().WithMessage("At least one permission must be specified.");

        RuleForEach(x => x.Permissions)
            .NotEmpty().WithMessage("Permission name cannot be empty.");

        RuleFor(x => x.Permissions)
            .MustAsync(async (list, ct) =>
            {
                var names = await permissions.GetAllNamesAsync(ct);
                return list.All(p => names.Contains(p));
            })
            .WithMessage("One or more permissions are not defined in the catalog.");
    }
}
