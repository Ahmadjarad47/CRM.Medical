using System.Security.Claims;
using CRM.Medical.Application.Abstractions;

namespace CRM.Medical.API.Services;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public IReadOnlyList<string> Roles =>
        _httpContextAccessor.HttpContext?.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
        ?? [];

    public bool IsInRole(string roleName) =>
        Roles.Any(r => string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase));
}
