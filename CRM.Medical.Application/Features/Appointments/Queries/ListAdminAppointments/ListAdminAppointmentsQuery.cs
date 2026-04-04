using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.ListAdminAppointments;

public sealed record ListAdminAppointmentsQuery(
    int Page,
    int PageSize,
    string? PatientId,
    string? DoctorId,
    string? LabPartnerId,
    string? Status) : IRequest<PagedResult<AppointmentDto>>;
