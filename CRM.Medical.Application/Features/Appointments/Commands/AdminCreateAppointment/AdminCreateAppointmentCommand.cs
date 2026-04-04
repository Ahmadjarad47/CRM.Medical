using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.AdminCreateAppointment;

public sealed record AdminCreateAppointmentCommand(
    string AdminUserId,
    string PatientId,
    string? DoctorId,
    string? LabPartnerId,
    AppointmentFormFields Fields) : IRequest<AppointmentDto>;
