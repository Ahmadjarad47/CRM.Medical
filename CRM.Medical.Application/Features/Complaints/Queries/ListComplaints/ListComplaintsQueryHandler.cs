using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Complaints;
using CRM.Medical.Application.Features.Complaints.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Queries.ListComplaints;

public sealed class ListComplaintsQueryHandler(IComplaintRepository complaints)
    : IRequestHandler<ListComplaintsQuery, PagedResult<ComplaintDto>>
{
    public async Task<PagedResult<ComplaintDto>> Handle(
        ListComplaintsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await complaints.ListAsync(
            request.UserId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<ComplaintDto>
        {
            Items = items.Select(c => c.ToDto()).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
