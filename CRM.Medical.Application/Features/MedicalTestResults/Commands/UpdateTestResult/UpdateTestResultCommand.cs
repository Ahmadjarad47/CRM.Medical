using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.UpdateTestResult;

public sealed record UpdateTestResultCommand(
    int Id,
    DateTime ResultDate,
    JsonElement? ResultData,
    string? PdfUrl,
    string Status) : IRequest<TestResultDto>;
