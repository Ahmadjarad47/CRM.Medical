using CRM.Medical.Application.Features.TestResults.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed class DeleteTestResultCommandHandler(ITestResultService testResults)
    : IRequestHandler<DeleteTestResultCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTestResultCommand request, CancellationToken cancellationToken)
    {
        await testResults.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
