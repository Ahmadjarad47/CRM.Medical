using CRM.Medical.Application.Features.Complaints.Commands.SubmitComplaint;

namespace CRM.Medical.API.Services.Complaints;

public interface IComplaintSubmitCommandFactory
{
    Task<SubmitComplaintCommand> CreateAsync(
        string userId,
        string title,
        string description,
        IFormFile? attachment,
        CancellationToken cancellationToken);
}

public sealed class ComplaintSubmitCommandFactory : IComplaintSubmitCommandFactory
{
    public async Task<SubmitComplaintCommand> CreateAsync(
        string userId,
        string title,
        string description,
        IFormFile? attachment,
        CancellationToken cancellationToken)
    {
        byte[]? fileBytes = null;
        string? contentType = null;
        string? fileName = null;

        if (attachment is { Length: > 0 })
        {
            await using var ms = new MemoryStream();
            await attachment.CopyToAsync(ms, cancellationToken);
            fileBytes = ms.ToArray();
            contentType = attachment.ContentType;
            fileName = attachment.FileName;
        }

        return new SubmitComplaintCommand(userId, title, description, fileBytes, contentType, fileName);
    }
}
