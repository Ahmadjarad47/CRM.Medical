using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.ListPatientAppointments;

public sealed record ListPatientAppointmentsQuery(string PatientUserId, int Page, int PageSize)
    : IRequest<PagedResult<AppointmentDto>>;
