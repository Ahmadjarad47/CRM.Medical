using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.WelcomePages.DTOs;

public sealed record WelcomePageDto(
    int Id,
    string Name,
    string Description,
    AdMediaType MediaType,
    string MediaUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
