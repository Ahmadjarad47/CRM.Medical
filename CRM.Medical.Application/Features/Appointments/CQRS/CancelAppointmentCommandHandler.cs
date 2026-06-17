using CRM.Medical.Application.Features.Appointments.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed class CancelAppointmentCommandHandler(IAppointmentService appointments)
    : IRequestHandler<CancelAppointmentCommand, Unit>
{
    public async Task<Unit> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        await appointments.CancelAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
