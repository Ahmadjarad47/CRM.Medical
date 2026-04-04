using CRM.Medical.Application.Features.Complaints.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Queries.GetMyComplaintById;

public sealed record GetMyComplaintByIdQuery(string UserId, int Id) : IRequest<ComplaintDto>;
