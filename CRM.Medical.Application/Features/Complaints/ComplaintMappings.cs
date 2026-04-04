using CRM.Medical.Application.Features.Complaints.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Complaints;

internal static class ComplaintMappings
{
    public static ComplaintDto ToDto(this Complaint c) =>
        new(
            c.Id,
            c.UserId,
            c.Title,
            c.Description,
            c.AttachmentUrl,
            c.Status,
            c.CreatedAt,
            c.UpdatedAt);
}
