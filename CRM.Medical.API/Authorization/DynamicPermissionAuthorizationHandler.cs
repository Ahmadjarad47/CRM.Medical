using System.Security.Claims;
using CRM.Medical.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Authorization;

public sealed class DynamicPermissionAuthorizationHandler(IPolicyEngine policyEngine)
    : AuthorizationHandler<DynamicPermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DynamicPermissionRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return;

        var resourceBag = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var requestBag = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        if (context.Resource is HttpContext httpContext)
        {
            foreach (var item in httpContext.Request.RouteValues)
                resourceBag[item.Key] = item.Value?.ToString();

            foreach (var item in httpContext.Request.Query)
                requestBag[item.Key] = item.Value.ToString();
        }

        var decision = await policyEngine.AuthorizeAsync(
            new PolicyEvaluationContext
            {
                UserId = userId,
                Roles = context.User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList(),
                Permission = requirement.Permission,
                Resource = resourceBag,
                Request = requestBag
            },
            CancellationToken.None);

        if (decision.IsAllowed)
            context.Succeed(requirement);
    }
}
