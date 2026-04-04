using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.ListDoctorAppointments;

public sealed record ListDoctorAppointmentsQuery(string DoctorUserId, int Page, int PageSize)
    : IRequest<PagedResult<AppointmentDto>>;
