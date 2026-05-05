using CRM.Medical.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Authorization;

public sealed class DynamicAuthorizeAttribute : AuthorizeAttribute
{
    public DynamicAuthorizeAttribute(string resource, string action)
    {
        Policy = new PermissionDescriptor(resource, action).Key;
    }
}
