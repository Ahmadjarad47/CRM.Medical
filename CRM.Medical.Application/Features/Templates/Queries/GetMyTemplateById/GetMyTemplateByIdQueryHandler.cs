using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Templates.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Templates.Queries.GetMyTemplateById;

public sealed class GetMyTemplateByIdQueryHandler(ITemplateRepository templates)
    : IRequestHandler<GetMyTemplateByIdQuery, TemplateDto>
{
    public async Task<TemplateDto> Handle(
        GetMyTemplateByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await templates.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Template '{request.Id}' was not found.");

        if (!string.Equals(entity.Role, request.Role, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You cannot access this template.");

        return entity.ToDto();
    }
}

