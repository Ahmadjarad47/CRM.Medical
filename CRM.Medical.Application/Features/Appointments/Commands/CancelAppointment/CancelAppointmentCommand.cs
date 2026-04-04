using CRM.Medical.Application.Features.Appointments;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.CancelAppointment;

public sealed record CancelAppointmentCommand(
    int AppointmentId,
    string ActingUserId,
    AppointmentCancellationActor Actor) : IRequest;
