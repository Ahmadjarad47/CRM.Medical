namespace CRM.Medical.Domain.Entities;

public sealed class ExternalPatient : BaseEntity
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public int? Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Optional identifier from partner systems.</summary>
    public string? ExternalId { get; set; }

    /// <summary>When set, associates this lab record with a registered portal patient user.</summary>
    public string? LinkedDirectPatientId { get; set; }

    public ICollection<TestRequest> TestRequests { get; set; } = new List<TestRequest>();

    public void LinkToDirectPatient(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        LinkedDirectPatientId = userId.Trim();
    }
}
