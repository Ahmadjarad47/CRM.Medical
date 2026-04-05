using System.Text.Json;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Commands.CreateTestRequest;

public sealed record CreateTestRequestCommand(
    int MedicalTestId,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonElement? Metadata,
    string CreatedByUserId) : IRequest<TestRequestDto>;
