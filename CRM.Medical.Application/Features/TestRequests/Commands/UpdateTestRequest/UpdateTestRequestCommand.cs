using System.Text.Json;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Commands.UpdateTestRequest;

public sealed record UpdateTestRequestCommand(
    int Id,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonElement? Metadata) : IRequest<TestRequestDto>;
