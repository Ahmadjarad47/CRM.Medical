using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Complaints;
using CRM.Medical.Application.Features.Complaints.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Queries.GetMyComplaintById;

public sealed class GetMyComplaintByIdQueryHandler(IComplaintRepository complaints)
    : IRequestHandler<GetMyComplaintByIdQuery, ComplaintDto>
{
    public async Task<ComplaintDto> Handle(
        GetMyComplaintByIdQuery request,
        CancellationToken cancellationToken)
    {
        var complaint = await complaints.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Complaint '{request.Id}' was not found.");

        if (!string.Equals(complaint.UserId, request.UserId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You cannot access this complaint.");

        return complaint.ToDto();
    }
}
