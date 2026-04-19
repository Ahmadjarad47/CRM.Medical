using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Templates.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Templates.Commands.CreateTemplate;

public sealed class CreateTemplateCommandHandler(
    ITemplateRepository templates,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateTemplateCommand, TemplateDto>
{
    public async Task<TemplateDto> Handle(
        CreateTemplateCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new Template
        {
            Name = request.Name.Trim(),
            Data = ProfileMetadataMapper.ToJsonDocument(request.Data),
            Role = request.Role,
            CreatedAt = dateTimeProvider.UtcNow
        };

        await templates.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}

