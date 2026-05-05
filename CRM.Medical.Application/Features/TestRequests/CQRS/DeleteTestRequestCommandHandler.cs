using CRM.Medical.Application.Features.TestRequests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed class DeleteTestRequestCommandHandler(ITestRequestService testRequests)
    : IRequestHandler<DeleteTestRequestCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTestRequestCommand request, CancellationToken cancellationToken)
    {
        await testRequests.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
