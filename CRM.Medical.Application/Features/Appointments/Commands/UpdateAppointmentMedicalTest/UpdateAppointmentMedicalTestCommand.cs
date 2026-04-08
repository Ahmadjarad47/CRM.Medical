using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.UpdateAppointmentMedicalTest;

public sealed record UpdateAppointmentMedicalTestCommand(
    int AppointmentId,
    int? MedicalTestId,
    string? MedicalTestCompletionStatus) : IRequest<AppointmentDto>;
