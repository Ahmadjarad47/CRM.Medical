using CRM.Medical.Application.Exceptions;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Commands.DeleteTestRequest;

public sealed class DeleteTestRequestCommandHandler(ITestRequestRepository repository)
    : IRequestHandler<DeleteTestRequestCommand>
{
    public async Task Handle(DeleteTestRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{request.Id}' was not found.");

        if (await repository.HasResultAsync(request.Id, cancellationToken))
            throw new ApplicationConflictException(
                "Cannot delete a test request that has a result. Delete the result first.");

        entity.Metadata?.Dispose();
        await repository.DeleteAsync(entity, cancellationToken);
    }
}
