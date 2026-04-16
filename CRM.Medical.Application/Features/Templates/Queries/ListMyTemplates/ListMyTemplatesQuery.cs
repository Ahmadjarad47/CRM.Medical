using CRM.Medical.Application.Features.Templates.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Templates.Queries.ListMyTemplates;

public sealed record ListMyTemplatesQuery(string UserId) : IRequest<IReadOnlyList<TemplateDto>>;

