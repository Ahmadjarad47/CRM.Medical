using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestResults.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed record ListTestResultsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    int? TestRequestId = null) : IRequest<PagedResult<TestResultDto>>;
