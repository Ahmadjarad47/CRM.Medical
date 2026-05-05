using CRM.Medical.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Authorization;

public sealed class DynamicPermissionRequirement(PermissionDescriptor permission) : IAuthorizationRequirement
{
    public PermissionDescriptor Permission { get; } = permission;
}
