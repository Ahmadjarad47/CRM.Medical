using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Queries.ListTestResults;

public sealed record ListTestResultsQuery(
    int Page,
    int PageSize,
    int? TestRequestId,
    string? Status) : IRequest<PagedResult<TestResultDto>>;
