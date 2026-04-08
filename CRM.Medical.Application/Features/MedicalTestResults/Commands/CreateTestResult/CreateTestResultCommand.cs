using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.CreateTestResult;

public sealed record CreateTestResultCommand(
    int TestRequestId,
    DateTime ResultDate,
    JsonElement? ResultData,
    string? PdfUrl,
    string Status,
    string CreatedByUserId) : IRequest<TestResultDto>;
