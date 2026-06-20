using CRM.Medical.Application.Features.WelcomePages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Queries.ListWebsiteWelcomePages;

public sealed class ListWebsiteWelcomePagesQueryHandler(IWelcomePageRepository welcomePages)
    : IRequestHandler<ListWebsiteWelcomePagesQuery, IReadOnlyList<WelcomePageDto>>
{
    public async Task<IReadOnlyList<WelcomePageDto>> Handle(
        ListWebsiteWelcomePagesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await welcomePages.ListActiveAsync(cancellationToken);
        return items.Select(w => w.ToDto()).ToList();
    }
}
