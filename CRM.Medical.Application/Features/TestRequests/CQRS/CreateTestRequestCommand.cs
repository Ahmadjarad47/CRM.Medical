using System.Text.Json;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed record CreateTestRequestCommand(
    IReadOnlyList<int> MedicalTestIds,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonDocument? Metadata,
    string? DoctorId,
    string? LabClientId,
    string? DirectPatientId,
    int? ExternalPatientId) : IRequest<IReadOnlyList<TestRequestDto>>;
