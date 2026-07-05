namespace CRM.Medical.Application.Abstractions;

internal sealed class AnonymousCurrentUserAccessor : ICurrentUserAccessor
{
    public string? UserId => null;
    public string? Email => null;
    public IReadOnlyList<string> Roles => [];
    public string? TenantId => null;

    public bool IsInRole(string roleName) => false;
}
