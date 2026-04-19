using CRM.Medical.Application.Features.Templates.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Templates.Queries.ListMyTemplates;

public sealed record ListMyTemplatesQuery(string Role) : IRequest<IReadOnlyList<TemplateDto>>;

