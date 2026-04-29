namespace CRM.Medical.Application.Features.Permissions.Services;

public interface IUserEffectivePermissionsProvider
{
    Task<IReadOnlyList<string>> GetPermissionNamesForUserAsync(
        string userId,
        CancellationToken cancellationToken);
}
