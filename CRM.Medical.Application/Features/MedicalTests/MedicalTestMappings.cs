using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.MedicalTests;

internal static class MedicalTestMappings
{
    public static MedicalTestDto ToDto(this MedicalTest t) =>
        new(
            t.Id,
            t.NameAr,
            t.NameEn,
            t.Price,
            t.Category,
            t.SampleType,
            ProfileMetadataMapper.ToJsonElement(t.ParameterSchema),
            t.Status,
            t.CreatedByUserId,
            t.CreatedAt,
            t.UpdatedAt);
}
