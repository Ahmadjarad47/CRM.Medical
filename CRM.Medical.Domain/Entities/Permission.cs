namespace CRM.Medical.Domain.Entities;

/// <summary>
/// Catalog of assignable permission names. Grants on users are stored as Identity user claims (claim type "permission").
/// </summary>
public sealed class Permission
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
