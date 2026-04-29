namespace CRM.Medical.Domain.Entities;

public sealed class UserPermission : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public Guid PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;
}
