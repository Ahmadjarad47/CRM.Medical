using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Queries.ListTestRequests;

public sealed class ListTestRequestsQueryHandler(ITestRequestRepository repository)
    : IRequestHandler<ListTestRequestsQuery, PagedResult<TestRequestDto>>
{
    public async Task<PagedResult<TestRequestDto>> Handle(
        ListTestRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(
            request.MedicalTestId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<TestRequestDto>
        {
            Items = items.Select(r => r.ToDto()).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
