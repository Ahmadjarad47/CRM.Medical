using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalTestResults;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using CRM.Medical.Application.Features.TestRequests;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.CreateTestResult;

public sealed class CreateTestResultCommandHandler(
    ITestResultRepository results,
    ITestRequestRepository testRequests,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateTestResultCommand, TestResultDto>
{
    public async Task<TestResultDto> Handle(
        CreateTestResultCommand request,
        CancellationToken cancellationToken)
    {
        _ = await testRequests.GetByIdAsync(request.TestRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{request.TestRequestId}' was not found.");

        if (await results.ExistsForTestRequestAsync(request.TestRequestId, cancellationToken))
            throw new ApplicationConflictException("A result already exists for this test request.");

        var now = dateTimeProvider.UtcNow;
        var entity = new TestResult
        {
            TestRequestId = request.TestRequestId,
            ResultDate = request.ResultDate,
            ResultData = ProfileMetadataMapper.ToJsonDocument(request.ResultData),
            PdfUrl = request.PdfUrl,
            Status = request.Status,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = now
        };

        await results.AddAsync(entity, cancellationToken);
        var loaded = await results.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{entity.Id}' was not found after create.");
        return loaded.ToDto();
    }
}
