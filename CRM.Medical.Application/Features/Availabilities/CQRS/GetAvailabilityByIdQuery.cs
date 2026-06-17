using CRM.Medical.Application.Features.Availabilities.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed record GetAvailabilityByIdQuery(int Id) : IRequest<AvailabilityDto>;
