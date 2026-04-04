using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.DoctorCreateAppointment;

public sealed record DoctorCreateAppointmentCommand(
    string DoctorUserId,
    string PatientId,
    string LabPartnerId,
    AppointmentFormFields Fields) : IRequest<AppointmentDto>;
