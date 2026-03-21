using MediatR;

namespace CRM.Medical.Application.Features.Health.GetStatus;

public sealed record GetHealthStatusQuery : IRequest<HealthStatusViewModel>;
