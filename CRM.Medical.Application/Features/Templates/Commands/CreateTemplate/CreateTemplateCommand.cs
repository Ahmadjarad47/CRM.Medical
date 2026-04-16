using System.Text.Json;
using CRM.Medical.Application.Features.Templates.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Templates.Commands.CreateTemplate;

public sealed record CreateTemplateCommand(
    string Name,
    JsonElement? Data,
    string UserId) : IRequest<TemplateDto>;

