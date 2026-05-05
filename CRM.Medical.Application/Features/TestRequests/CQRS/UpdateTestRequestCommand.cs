using System.Text.Json;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed record UpdateTestRequestCommand(
    int Id,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonDocument? Metadata,
    string? DoctorId,
    string? LabClientId,
    string? DirectPatientId,
    int? ExternalPatientId) : IRequest<Unit>;
