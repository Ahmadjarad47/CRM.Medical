namespace CRM.Medical.Application.Features.Permissions.DTOs;

public sealed record PermissionDto(Guid Id, string Name, string? Description, DateTime CreatedAt);
