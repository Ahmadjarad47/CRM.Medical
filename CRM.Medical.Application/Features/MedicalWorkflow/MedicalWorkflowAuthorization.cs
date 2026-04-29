using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;

namespace CRM.Medical.Application.Features.MedicalWorkflow;

/// <summary>
/// Central permission and patient read-only rules for medical test workflow services.
/// Data scope filtering stays in services that query entities.
/// </summary>
public static class MedicalWorkflowAuthorization
{
    public static void RequireAuthenticatedUser(ICurrentUserAccessor user)
    {
        if (string.IsNullOrEmpty(user.UserId))
            throw new ApplicationUnauthorizedException("Unable to identify the current user.");
    }

    public static void RequirePermissionOrAdmin(ICurrentUserAccessor user, string permissionName)
    {
        if (user.IsInRole(UserRoles.Admin))
            return;

        if (!user.HasPermission(permissionName))
            throw new ApplicationForbiddenException($"Required permission: {permissionName}.");
    }

    public static void DenyPatientMutations(ICurrentUserAccessor user)
    {
        if (user.IsInRole(UserRoles.Patient))
            throw new ApplicationForbiddenException("Patients have read-only access to this resource.");
    }
}
