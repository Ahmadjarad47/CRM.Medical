using System.Security.Claims;
using CRM.Medical.Application.Abstractions;

namespace CRM.Medical.API.Services;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserAccessor
{
    public string? UserId =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}
