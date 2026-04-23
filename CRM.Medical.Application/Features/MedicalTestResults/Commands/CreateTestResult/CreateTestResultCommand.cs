using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.CreateTestResult;

public sealed record CreateTestResultCommand(
    int TestRequestId,
    DateTime ResultDate,
    JsonElement? ResultData,
    IFormFile? PdfFile,
    string Status,
    string CreatedByUserId) : IRequest<TestResultDto>;
