using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed record ListTestRequestsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null) : IRequest<PagedResult<TestRequestDto>>;
