using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(RoleManager<IdentityRole> roleManager)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync(async (name, ct) => !await roleManager.RoleExistsAsync(name))
            .WithMessage("A role with this name already exists.");
    }
}
