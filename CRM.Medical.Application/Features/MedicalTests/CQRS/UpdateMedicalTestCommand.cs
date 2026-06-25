using System.Text.Json;
using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed record UpdateMedicalTestCommand(
    int Id,
    string NameAr,
    string NameEn,
    double Price,
    int CategoryMedicalId,
    string SampleType,
    JsonDocument? ParameterSchema,
    MedicalTestStatus Status) : IRequest<Unit>;
