using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Complaints;
using CRM.Medical.Application.Features.Complaints.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Commands.SubmitComplaint;

public sealed class SubmitComplaintCommandHandler(
    IComplaintRepository complaints,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<SubmitComplaintCommand, ComplaintDto>
{
    public async Task<ComplaintDto> Handle(SubmitComplaintCommand request, CancellationToken cancellationToken)
    {
        string? attachmentUrl = null;

        if (request.Attachment is { Length: > 0 })
            attachmentUrl = await fileStorage.UploadFileAsync(request.Attachment, "complaints", cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var entity = new Complaint
        {
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description,
            AttachmentUrl = attachmentUrl,
            Status = ComplaintStatuses.Pending,
            CreatedAt = now
        };

        await complaints.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
