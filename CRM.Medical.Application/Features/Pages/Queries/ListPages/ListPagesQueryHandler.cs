using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Queries.ListPages;

public sealed class ListPagesQueryHandler(IPageRepository pages)
    : IRequestHandler<ListPagesQuery, IReadOnlyList<PageListItemDto>>
{
    public async Task<IReadOnlyList<PageListItemDto>> Handle(ListPagesQuery request, CancellationToken cancellationToken)
    {
        var items = await pages.ListAsync(cancellationToken);
        return items.Select(p => p.ToListItemDto()).ToList();
    }
}
