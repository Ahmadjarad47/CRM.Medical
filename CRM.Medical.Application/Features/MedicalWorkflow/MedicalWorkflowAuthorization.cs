using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;

namespace CRM.Medical.Application.Features.MedicalWorkflow;

/// <summary>
/// Central authorization helpers for medical workflow services (evaluated via <see cref="IPolicyEngine"/>).
/// Data scope filtering stays in services that query entities.
/// </summary>
public static class MedicalWorkflowAuthorization
{
    public static void RequireAuthenticatedUser(ICurrentUserAccessor user)
    {
        if (string.IsNullOrEmpty(user.UserId))
            throw new ApplicationUnauthorizedException("Unable to identify the current user.");
    }

    /// <summary>
    /// Enforces <c>Resource:Action</c> through ABAC policies, except users in the Admin role (same bypass as before).
    /// </summary>
    public static async Task RequireAccessOrAdminAsync(
        ICurrentUserAccessor user,
        IPolicyEngine policyEngine,
        string resource,
        string action,
        CancellationToken cancellationToken)
    {
        RequireAuthenticatedUser(user);
        if (user.IsInRole(UserRoles.Admin))
            return;

        var decision = await policyEngine.AuthorizeAsync(
            new PolicyEvaluationContext
            {
                UserId = user.UserId!,
                Roles = user.Roles.ToList(),
                Permission = new PermissionDescriptor(resource, action),
                Resource = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase),
                Request = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            },
            cancellationToken);

        if (!decision.IsAllowed)
            throw new ApplicationForbiddenException($"Required access: {resource}:{action}.");
    }

    public static void DenyPatientMutations(ICurrentUserAccessor user)
    {
        if (user.IsInRole(UserRoles.Patient))
            throw new ApplicationForbiddenException("Patients have read-only access to this resource.");
    }
}
