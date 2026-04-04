namespace CRM.Medical.Application.Features.Complaints.DTOs;

public sealed record ComplaintDto(
    int Id,
    string UserId,
    string Title,
    string Description,
    string? AttachmentUrl,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
