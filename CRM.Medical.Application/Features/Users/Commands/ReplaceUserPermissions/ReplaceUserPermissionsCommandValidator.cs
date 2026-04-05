using CRM.Medical.Application.Abstractions;
using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.ReplaceUserPermissions;

public sealed class ReplaceUserPermissionsCommandValidator : AbstractValidator<ReplaceUserPermissionsCommand>
{
    public ReplaceUserPermissionsCommandValidator(IPermissionRepository permissions)
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Permissions)
            .MustAsync(async (list, ct) =>
            {
                if (list.Count == 0)
                    return true;

                var names = await permissions.GetAllNamesAsync(ct);
                return list.All(p => names.Contains(p));
            })
            .WithMessage("One or more permissions are not defined in the catalog.");

        RuleForEach(x => x.Permissions)
            .NotEmpty()
            .When(x => x.Permissions.Count > 0);
    }
}
