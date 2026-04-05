using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Queries.ListTestRequests;

public sealed record ListTestRequestsQuery(
    int Page,
    int PageSize,
    int? MedicalTestId,
    string? Status) : IRequest<PagedResult<TestRequestDto>>;
