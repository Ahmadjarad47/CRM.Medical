using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Complaints;
using CRM.Medical.Application.Features.Complaints.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Queries.GetMyComplaints;

public sealed class GetMyComplaintsQueryHandler(IComplaintRepository complaints)
    : IRequestHandler<GetMyComplaintsQuery, PagedResult<ComplaintDto>>
{
    public async Task<PagedResult<ComplaintDto>> Handle(
        GetMyComplaintsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await complaints.ListAsync(
            request.UserId,
            status: null,
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
