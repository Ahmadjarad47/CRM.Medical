using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed record CreateMedicalTestCommand(
    string NameAr,
    string NameEn,
    double Price,
    int CategoryMedicalId,
    string SampleType,
    JsonDocument? ParameterSchema,
    MedicalTestStatus Status) : IRequest<MedicalTestDto>;
