using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.TestRequests;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.DeleteTestResult;

public sealed class DeleteTestResultCommandHandler(ITestResultRepository repository)
    : IRequestHandler<DeleteTestResultCommand>
{
    public async Task Handle(DeleteTestResultCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{request.Id}' was not found.");

        entity.ResultData?.Dispose();
        await repository.DeleteAsync(entity, cancellationToken);
    }
}
