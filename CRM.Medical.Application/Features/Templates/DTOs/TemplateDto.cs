using System.Text.Json;

namespace CRM.Medical.Application.Features.Templates.DTOs;

public sealed record TemplateDto(
    int Id,
    string Name,
    JsonElement? Data,
    string UserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

