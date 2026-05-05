using CRM.Medical.Application.Features.TestResults.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed class UpdateTestResultCommandHandler(ITestResultService testResults)
    : IRequestHandler<UpdateTestResultCommand, Unit>
{
    public async Task<Unit> Handle(UpdateTestResultCommand request, CancellationToken cancellationToken)
    {
        await testResults.UpdateAsync(
            request.Id,
            request.ResultDate,
            request.ResultData,
            request.PdfUrl,
            request.Status,
            cancellationToken);
        return Unit.Value;
    }
}
