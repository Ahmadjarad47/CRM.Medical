using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.User.Complaints;

public sealed class SubmitComplaintRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IFormFile? Attachment { get; init; }
}
