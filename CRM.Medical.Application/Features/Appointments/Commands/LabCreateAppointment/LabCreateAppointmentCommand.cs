using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.LabCreateAppointment;

public sealed record LabCreateAppointmentCommand(
    string LabUserId,
    string PatientId,
    string DoctorId,
    AppointmentFormFields Fields) : IRequest<AppointmentDto>;
