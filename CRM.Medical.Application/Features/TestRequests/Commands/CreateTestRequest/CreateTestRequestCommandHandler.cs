using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalTests;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Commands.CreateTestRequest;

public sealed class CreateTestRequestCommandHandler(
    ITestRequestRepository testRequests,
    IMedicalTestRepository medicalTests,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateTestRequestCommand, TestRequestDto>
{
    public async Task<TestRequestDto> Handle(
        CreateTestRequestCommand request,
        CancellationToken cancellationToken)
    {
        _ = await medicalTests.GetByIdAsync(request.MedicalTestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{request.MedicalTestId}' was not found.");

        if (await testRequests.ExistsForMedicalTestAsync(request.MedicalTestId, cancellationToken))
            throw new ApplicationConflictException(
                "A test request already exists for this medical test (one request per test).");

        var now = dateTimeProvider.UtcNow;
        var entity = new TestRequest
        {
            MedicalTestId = request.MedicalTestId,
            RequestDate = request.RequestDate,
            Status = request.Status,
            TotalAmount = request.TotalAmount,
            Notes = request.Notes,
            Metadata = ProfileMetadataMapper.ToJsonDocument(request.Metadata),
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = now
        };

        await testRequests.AddAsync(entity, cancellationToken);
        var loaded = await testRequests.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{entity.Id}' was not found after create.");
        return loaded.ToDto();
    }
}
