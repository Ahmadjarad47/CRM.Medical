using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.RemoveUserPermissions;

public sealed class RemoveUserPermissionsCommandValidator : AbstractValidator<RemoveUserPermissionsCommand>
{
    public RemoveUserPermissionsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Permissions).NotNull().NotEmpty();
        RuleForEach(x => x.Permissions).NotEmpty().MaximumLength(256);
    }
}
