using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed class GetTestResultByIdQueryHandler(ITestResultService testResults)
    : IRequestHandler<GetTestResultByIdQuery, TestResultDto>
{
    public Task<TestResultDto> Handle(
        GetTestResultByIdQuery request,
        CancellationToken cancellationToken) =>
        testResults.GetByIdAsync(request.Id, cancellationToken);
}
