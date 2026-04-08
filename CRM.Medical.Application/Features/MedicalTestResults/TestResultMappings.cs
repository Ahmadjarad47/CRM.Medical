using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.MedicalTestResults;

internal static class TestResultMappings
{
    public static TestResultDto ToDto(this TestResult r) =>
        new(
            r.Id,
            r.TestRequestId,
            r.TestRequest.MedicalTestId,
            r.TestRequest.MedicalTest.NameEn,
            r.ResultDate,
            ProfileMetadataMapper.ToJsonElement(r.ResultData),
            r.PdfUrl,
            r.Status,
            r.CreatedByUserId,
            r.CreatedAt,
            r.UpdatedAt);
}
