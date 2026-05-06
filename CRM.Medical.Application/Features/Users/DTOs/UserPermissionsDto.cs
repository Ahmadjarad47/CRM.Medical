using CRM.Medical.Application.Features.Permissions.DTOs;

namespace CRM.Medical.Application.Features.Users.DTOs;

/// <summary>Allow policies from <c>access_policies</c> that apply to the user (by subject user/role/group).</summary>
public sealed record UserPermissionsDto(
    string UserId,
    IReadOnlyList<AccessPolicyDto> AccessPolicies);
