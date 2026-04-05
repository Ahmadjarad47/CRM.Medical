using CRM.Medical.Application.Abstractions;
using FluentValidation;

namespace CRM.Medical.Application.Features.Permissions.Commands.CreatePermission;

public sealed class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator(IPermissionRepository permissions)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(128)
            .Matches(@"^[a-z][a-z0-9._-]*$")
            .WithMessage("Name must start with a lowercase letter and contain only lowercase letters, digits, dots, underscores, or hyphens.")
            .MustAsync(async (name, ct) => await permissions.GetByNameAsync(name, ct) is null)
            .WithMessage("A permission with this name already exists.");
    }
}
