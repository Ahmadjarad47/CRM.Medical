using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed class ListTestRequestsQueryHandler(ITestRequestService testRequests)
    : IRequestHandler<ListTestRequestsQuery, PagedResult<TestRequestDto>>
{
    public Task<PagedResult<TestRequestDto>> Handle(
        ListTestRequestsQuery request,
        CancellationToken cancellationToken) =>
        testRequests.ListAsync(request.Page, request.PageSize, request.Search, cancellationToken);
}
