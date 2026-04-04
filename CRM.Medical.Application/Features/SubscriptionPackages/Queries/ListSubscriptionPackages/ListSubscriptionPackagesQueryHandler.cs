using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.SubscriptionPackages;
using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Queries.ListSubscriptionPackages;

public sealed class ListSubscriptionPackagesQueryHandler(ISubscriptionPackageRepository repository)
    : IRequestHandler<ListSubscriptionPackagesQuery, PagedResult<SubscriptionPackageDto>>
{
    public async Task<PagedResult<SubscriptionPackageDto>> Handle(
        ListSubscriptionPackagesQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(
            request.TargetAudience,
            request.IsActive,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<SubscriptionPackageDto>
        {
            Items = items.Select(p => p.ToDto()).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
