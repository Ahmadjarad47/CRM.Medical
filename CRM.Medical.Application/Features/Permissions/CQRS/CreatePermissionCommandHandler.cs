using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed class CreateAccessPolicyCommandHandler(IAccessPolicyService accessPolicyService)
    : IRequestHandler<CreateAccessPolicyCommand, AccessPolicyDto>
{
    public Task<AccessPolicyDto> Handle(CreateAccessPolicyCommand request, CancellationToken cancellationToken) =>
        accessPolicyService.CreateAsync(
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
}
