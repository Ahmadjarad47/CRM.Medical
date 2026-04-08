using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.TestRequests;

internal static class TestRequestMappings
{
    public static TestRequestDto ToDto(this TestRequest r) =>
        new(
            r.Id,
            r.MedicalTestId,
            r.MedicalTest.NameEn,
            r.RequestDate,
            r.Status,
            r.TotalAmount,
            r.Notes,
            ProfileMetadataMapper.ToJsonElement(r.Metadata),
            r.CreatedByUserId,
            r.CreatedAt,
            r.UpdatedAt,
            r.Result?.Id);
}
