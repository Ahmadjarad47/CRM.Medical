using System.Text.Json;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed record CreateTestRequestCommand(
    int MedicalTestId,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonDocument? Metadata,
    string? DoctorId,
    string? LabClientId,
    string? DirectPatientId,
    int? ExternalPatientId) : IRequest<TestRequestDto>;
