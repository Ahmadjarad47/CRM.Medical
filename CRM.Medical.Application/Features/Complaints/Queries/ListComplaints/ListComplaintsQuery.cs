using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Complaints.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Queries.ListComplaints;

public sealed record ListComplaintsQuery(
    int Page,
    int PageSize,
    string? Status,
    string? UserId) : IRequest<PagedResult<ComplaintDto>>;
