using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.AddUserPermissions;

public sealed class AddUserPermissionsCommandValidator : AbstractValidator<AddUserPermissionsCommand>
{
    public AddUserPermissionsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Permissions).NotNull().NotEmpty();
        RuleForEach(x => x.Permissions).NotEmpty().MaximumLength(256);
    }
}
