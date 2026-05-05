using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed class CreateTestResultCommandHandler(ITestResultService testResults)
    : IRequestHandler<CreateTestResultCommand, TestResultDto>
{
    public Task<TestResultDto> Handle(
        CreateTestResultCommand request,
        CancellationToken cancellationToken) =>
        testResults.CreateAsync(
            request.TestRequestId,
            request.ResultDate,
            request.ResultData,
            request.PdfUrl,
            request.Status,
            cancellationToken);
}
