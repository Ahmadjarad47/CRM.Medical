using System.Text.Json;
using CRM.Medical.Application.Features.TestResults.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed record CreateTestResultCommand(
    int TestRequestId,
    DateTime ResultDate,
    JsonDocument? ResultData,
    string? PdfUrl,
    string Status) : IRequest<TestResultDto>;
