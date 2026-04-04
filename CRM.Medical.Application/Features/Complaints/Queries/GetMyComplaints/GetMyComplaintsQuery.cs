using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Complaints.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Queries.GetMyComplaints;

public sealed record GetMyComplaintsQuery(string UserId, int Page, int PageSize)
    : IRequest<PagedResult<ComplaintDto>>;
