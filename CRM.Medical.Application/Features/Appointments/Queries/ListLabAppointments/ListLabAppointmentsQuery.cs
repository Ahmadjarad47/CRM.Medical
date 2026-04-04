using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.ListLabAppointments;

public sealed record ListLabAppointmentsQuery(string LabUserId, int Page, int PageSize)
    : IRequest<PagedResult<AppointmentDto>>;
