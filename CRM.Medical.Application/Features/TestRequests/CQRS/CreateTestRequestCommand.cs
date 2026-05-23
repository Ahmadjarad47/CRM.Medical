using System.Text.Json;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed record CreateTestRequestItemCommand(
    int MedicalTestId,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonDocument? Metadata,
    string? DoctorId,
    string? LabClientId,
    string? DirectPatientId,
    int? ExternalPatientId);

public sealed record CreateTestRequestCommand(
    IReadOnlyList<CreateTestRequestItemCommand> Items) : IRequest<IReadOnlyList<TestRequestDto>>;
