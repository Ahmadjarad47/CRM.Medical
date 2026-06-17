using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed record DeleteAvailabilityCommand(int Id) : IRequest<Unit>;
