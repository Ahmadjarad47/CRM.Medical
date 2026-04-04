using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Complaints;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Commands.UpdateComplaintStatus;

public sealed class UpdateComplaintStatusCommandHandler(
    IComplaintRepository complaints,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateComplaintStatusCommand>
{
    public async Task Handle(UpdateComplaintStatusCommand request, CancellationToken cancellationToken)
    {
        var complaint = await complaints.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Complaint '{request.Id}' was not found.");

        complaint.Status = request.Status;
        complaint.UpdatedAt = dateTimeProvider.UtcNow;
        await complaints.UpdateAsync(complaint, cancellationToken);
    }
}
