using System.Text.Json;
using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.MedicalTests.DTOs;

public sealed record MedicalTestDto(
    int Id,
    string NameAr,
    string NameEn,
    double Price,
    int CategoryMedicalId,
    string CategoryNameAr,
    string CategoryNameEn,
    string SampleType,
    JsonElement? ParameterSchema,
    MedicalTestStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
