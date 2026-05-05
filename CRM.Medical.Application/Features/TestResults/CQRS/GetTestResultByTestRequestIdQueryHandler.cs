using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed class GetTestResultByTestRequestIdQueryHandler(ITestResultService testResults)
    : IRequestHandler<GetTestResultByTestRequestIdQuery, TestResultDto>
{
    public Task<TestResultDto> Handle(
        GetTestResultByTestRequestIdQuery request,
        CancellationToken cancellationToken) =>
        testResults.GetByTestRequestIdAsync(request.TestRequestId, cancellationToken);
}
