using CRM.Medical.Application.Features.Permissions.DTOs;

namespace CRM.Medical.Application.Features.Permissions.Services;

/// <summary>
/// Resolves enabled allow policies that apply to a user (by user id and role names) from <c>access_policies</c>.
/// </summary>
public interface IUserEffectiveAccessPoliciesProvider
{
    Task<IReadOnlyList<AccessPolicyDto>> GetEffectiveAllowPoliciesForUserAsync(
        string userId,
        CancellationToken cancellationToken);
}
