using CRM.Medical.Application.Features.Permissions.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed class UpdateAccessPolicyCommandHandler(IAccessPolicyService accessPolicyService)
    : IRequestHandler<UpdateAccessPolicyCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAccessPolicyCommand request, CancellationToken cancellationToken)
    {
        await accessPolicyService.UpdateAsync(
            request.Id,
            request.Name,
            request.Resource,
            request.Action,
            request.SubjectType,
            request.SubjectId,
            request.Effect,
            request.Priority,
            request.ConditionJson,
            request.Description,
            request.IsEnabled,
            cancellationToken);
        return Unit.Value;
    }
}
