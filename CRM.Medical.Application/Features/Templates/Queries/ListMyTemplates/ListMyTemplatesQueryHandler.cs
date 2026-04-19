using CRM.Medical.Application.Features.Templates.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Templates.Queries.ListMyTemplates;

public sealed class ListMyTemplatesQueryHandler(ITemplateRepository templates)
    : IRequestHandler<ListMyTemplatesQuery, IReadOnlyList<TemplateDto>>
{
    public async Task<IReadOnlyList<TemplateDto>> Handle(
        ListMyTemplatesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await templates.ListByRoleAsync(request.Role, cancellationToken);
        return items.Select(t => t.ToDto()).ToList();
    }
}

