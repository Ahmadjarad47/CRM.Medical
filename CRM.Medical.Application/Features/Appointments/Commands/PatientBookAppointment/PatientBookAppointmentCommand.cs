using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.PatientBookAppointment;

public sealed record PatientBookAppointmentCommand(
    string PatientUserId,
    AppointmentFormFields Fields,
    string? DoctorId,
    string? LabPartnerId) : IRequest<AppointmentDto>;
