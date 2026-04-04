using CRM.Medical.Application.Features.Appointments;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.ConfirmAppointment;

public sealed record ConfirmAppointmentCommand(
    int AppointmentId,
    string ActingUserId,
    AppointmentConfirmationActor Actor) : IRequest;
