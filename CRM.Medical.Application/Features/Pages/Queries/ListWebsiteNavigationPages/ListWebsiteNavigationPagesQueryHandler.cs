using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Queries.ListWebsiteNavigationPages;

public sealed class ListWebsiteNavigationPagesQueryHandler(
    IPageRepository pages,
    IDateTimeProvider dateTimeProvider,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<ListWebsiteNavigationPagesQuery, IReadOnlyList<WebsiteNavigationPageDto>>
{
    public async Task<IReadOnlyList<WebsiteNavigationPageDto>> Handle(
        ListWebsiteNavigationPagesQuery request,
        CancellationToken cancellationToken)
    {
        var language = PageFeatureHelpers.NormalizeLanguage(request.Language);
        var items = await pages.ListPublishedForNavigationAsync(
            language,
            dateTimeProvider.UtcNow,
            cancellationToken);

        return items
            .Where(p => PageFeatureHelpers.IsVisibleToUser(p.VisibleToRoles, currentUser.Roles))
            .Select(p => p.ToWebsiteNavigationDto(language))
            .ToList();
    }
}
