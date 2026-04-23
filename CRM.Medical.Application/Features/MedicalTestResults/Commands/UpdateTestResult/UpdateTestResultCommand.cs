using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.UpdateTestResult;

public sealed record UpdateTestResultCommand(
    int Id,
    DateTime ResultDate,
    JsonElement? ResultData,
    IFormFile? PdfFile,
    string Status) : IRequest<TestResultDto>;
