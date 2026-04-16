using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Features.Templates.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Templates;

internal static class TemplateMappings
{
    public static TemplateDto ToDto(this Template t) =>
        new(
            t.Id,
            t.Name,
            ProfileMetadataMapper.ToJsonElement(t.Data),
            t.UserId,
            t.CreatedAt,
            t.UpdatedAt);
}

