using System.Text.Json;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed record UpdateMedicalTestCommand(
    int Id,
    string NameAr,
    string NameEn,
    double Price,
    string Category,
    string SampleType,
    JsonDocument? ParameterSchema,
    string Status) : IRequest<Unit>;
