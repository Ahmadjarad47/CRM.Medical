using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Queries.GetWebsitePageBySlug;

public sealed class GetWebsitePageBySlugQueryHandler(
    IPageRepository pages,
    IDateTimeProvider dateTimeProvider,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<GetWebsitePageBySlugQuery, WebsitePageDto>
{
    public async Task<WebsitePageDto> Handle(GetWebsitePageBySlugQuery request, CancellationToken cancellationToken)
    {
        var language = PageFeatureHelpers.NormalizeLanguage(request.Language);
        var slug = PageFeatureHelpers.NormalizeSlug(request.Slug);

        var entity = await pages.GetPublishedBySlugAsync(
            language,
            slug,
            dateTimeProvider.UtcNow,
            cancellationToken);

        if (entity is null || !PageFeatureHelpers.IsVisibleToUser(entity.VisibleToRoles, currentUser.Roles))
            throw new ApplicationNotFoundException($"Published page with slug '{slug}' was not found.");

        return entity.ToWebsiteDto(language);
    }
}
