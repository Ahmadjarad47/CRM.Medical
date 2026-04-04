using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Commands.UpdateComplaintStatus;

public sealed record UpdateComplaintStatusCommand(int Id, string Status) : IRequest;
