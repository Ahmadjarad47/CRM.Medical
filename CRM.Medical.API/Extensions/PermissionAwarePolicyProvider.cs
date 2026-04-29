using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CRM.Medical.API.Extensions;

/// <summary>
/// Delegates to the default policy provider first, then treats any other policy name as a user permission claim
/// (<see cref="UserPermissions.ClaimType"/>), so new catalog entries work without static <c>AddPolicy</c> calls.
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

        // Dynamic permission policies: JWT must carry the permission claim, unless the user is Admin
        // (full API access; row-level rules are still enforced in application services).
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(ctx =>
                ctx.User.IsInRole(UserRoles.Admin)
                || ctx.User.HasClaim(UserPermissions.ClaimType, policyName))
            .Build();
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        _fallback.GetFallbackPolicyAsync();
}
