using FluentValidation;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;

public sealed class GetUserPermissionsQueryValidator : AbstractValidator<GetUserPermissionsQuery>
{
    public GetUserPermissionsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
