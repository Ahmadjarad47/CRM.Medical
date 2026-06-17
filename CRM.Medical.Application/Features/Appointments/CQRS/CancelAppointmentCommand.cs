using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record CancelAppointmentCommand(int Id) : IRequest<Unit>;
