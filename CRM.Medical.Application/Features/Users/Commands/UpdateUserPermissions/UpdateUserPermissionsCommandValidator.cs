using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUserPermissions;

public sealed class UpdateUserPermissionsCommandValidator : AbstractValidator<UpdateUserPermissionsCommand>
{
    public UpdateUserPermissionsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Permissions).NotNull();
        RuleForEach(x => x.Permissions).NotEmpty().MaximumLength(256);
    }
}
