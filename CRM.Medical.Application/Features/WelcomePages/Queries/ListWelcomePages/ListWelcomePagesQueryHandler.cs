using CRM.Medical.Application.Features.WelcomePages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Queries.ListWelcomePages;

public sealed class ListWelcomePagesQueryHandler(IWelcomePageRepository welcomePages)
    : IRequestHandler<ListWelcomePagesQuery, IReadOnlyList<WelcomePageDto>>
{
    public async Task<IReadOnlyList<WelcomePageDto>> Handle(
        ListWelcomePagesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await welcomePages.ListAsync(cancellationToken);
        return items.Select(w => w.ToDto()).ToList();
    }
}
