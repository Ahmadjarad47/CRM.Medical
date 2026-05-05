using CRM.Medical.API.Authorization;
using CRM.Medical.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CRM.Medical.API.Extensions;

/// <summary>
/// Delegates to the default policy provider first, then treats any other policy name as a dynamic
/// ABAC permission key (Resource:Action) evaluated by <see cref="DynamicPermissionAuthorizationHandler"/>.
/// </summary>
public sealed class PermissionAwarePolicyProvider(IOptions<AuthorizationOptions> options)
    : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await _fallback.GetPolicyAsync(policyName);
        if (policy is not null)
            return policy;

        PermissionDescriptor permission;
        try
        {
            permission = PermissionDescriptor.FromPolicyName(policyName);
        }
        catch (ArgumentException)
        {
            return null;
        }

        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new DynamicPermissionRequirement(permission))
            .Build();
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        _fallback.GetFallbackPolicyAsync();
}
