using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Commands.SetSubscriptionPackageActive;

public sealed record SetSubscriptionPackageActiveCommand(int Id, bool IsActive) : IRequest<SubscriptionPackageDto>;
