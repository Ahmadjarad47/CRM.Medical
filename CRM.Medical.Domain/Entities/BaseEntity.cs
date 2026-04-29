namespace CRM.Medical.Domain.Entities;

/// <summary>
/// Shared audit fields for domain entities (not used by <see cref="User"/>, which inherits Identity types).
/// </summary>
public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedByUserId { get; set; }
}
