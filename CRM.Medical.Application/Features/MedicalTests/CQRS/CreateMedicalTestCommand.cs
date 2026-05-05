using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed record CreateMedicalTestCommand(
    string NameAr,
    string NameEn,
    double Price,
    string Category,
    string SampleType,
    JsonDocument? ParameterSchema,
    string Status) : IRequest<MedicalTestDto>;
