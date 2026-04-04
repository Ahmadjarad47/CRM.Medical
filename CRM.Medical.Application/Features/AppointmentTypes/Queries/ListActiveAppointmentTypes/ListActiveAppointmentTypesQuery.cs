using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Queries.ListActiveAppointmentTypes;

public sealed record ListActiveAppointmentTypesQuery : IRequest<IReadOnlyList<AppointmentTypeDto>>;
