using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Queries.GetSubscriptionPackageById;

public sealed record GetSubscriptionPackageByIdQuery(int Id) : IRequest<SubscriptionPackageDto>;
