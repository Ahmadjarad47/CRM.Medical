using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Queries.ListAdminAppointmentTypes;

public sealed record ListAdminAppointmentTypesQuery : IRequest<IReadOnlyList<AppointmentTypeDto>>;
