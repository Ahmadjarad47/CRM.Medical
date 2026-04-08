using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalTestResults;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using CRM.Medical.Application.Features.TestRequests;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Queries.GetTestResultById;

public sealed class GetTestResultByIdQueryHandler(ITestResultRepository repository)
    : IRequestHandler<GetTestResultByIdQuery, TestResultDto>
{
    public async Task<TestResultDto> Handle(
        GetTestResultByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{request.Id}' was not found.");

        return entity.ToDto();
    }
}
