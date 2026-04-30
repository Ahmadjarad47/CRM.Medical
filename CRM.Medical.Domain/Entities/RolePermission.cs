namespace CRM.Medical.Domain.Entities;

public sealed class RolePermission : BaseEntity
{
    public string RoleId { get; set; } = string.Empty;

    public Guid PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;
}
