using FluentValidation;

namespace CRM.Medical.Application.Features.Permissions.Commands.DeletePermission;

public sealed class DeletePermissionCommandValidator : AbstractValidator<DeletePermissionCommand>
{
    public DeletePermissionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
