using CRM.Medical.Application.Features.Templates.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Templates.Queries.GetMyTemplateById;

public sealed record GetMyTemplateByIdQuery(string Role, int Id) : IRequest<TemplateDto>;

