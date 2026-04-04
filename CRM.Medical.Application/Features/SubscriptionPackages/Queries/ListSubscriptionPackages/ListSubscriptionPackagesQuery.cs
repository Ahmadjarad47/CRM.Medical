using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Queries.ListSubscriptionPackages;

public sealed record ListSubscriptionPackagesQuery(
    int Page,
    int PageSize,
    SubscriptionPackageTargetAudience? TargetAudience,
    bool? IsActive) : IRequest<PagedResult<SubscriptionPackageDto>>;
