using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed class CreateTestRequestCommandHandler(ITestRequestService testRequests)
    : IRequestHandler<CreateTestRequestCommand, TestRequestDto>
{
    public Task<TestRequestDto> Handle(
        CreateTestRequestCommand request,
        CancellationToken cancellationToken) =>
        testRequests.CreateAsync(
            request.MedicalTestId,
            request.RequestDate,
            request.Status,
            request.TotalAmount,
            request.Notes,
            request.Metadata,
            request.DoctorId,
            request.LabClientId,
            request.DirectPatientId,
            request.ExternalPatientId,
            cancellationToken);
}
