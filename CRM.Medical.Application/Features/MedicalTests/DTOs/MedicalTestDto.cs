using System.Text.Json;

namespace CRM.Medical.Application.Features.MedicalTests.DTOs;

public sealed record MedicalTestDto(
    int Id,
    string NameAr,
    string NameEn,
    double Price,
    string Category,
    string SampleType,
    JsonElement? ParameterSchema,
    string Status,
    string CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
