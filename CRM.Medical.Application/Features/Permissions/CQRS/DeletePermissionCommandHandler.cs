using CRM.Medical.Application.Features.Permissions.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed class DeleteAccessPolicyCommandHandler(IAccessPolicyService accessPolicyService)
    : IRequestHandler<DeleteAccessPolicyCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccessPolicyCommand request, CancellationToken cancellationToken)
    {
        await accessPolicyService.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
