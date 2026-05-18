using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CRM.Medical.Application.Abstractions;

namespace CRM.Medical.API.Services;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");

    public string? Email =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("email");

    public IReadOnlyList<string> Roles =>
        _httpContextAccessor.HttpContext?.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
        ?? [];

    public string? TenantId =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue("tenant_id")
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("tenantId")
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("tid");

    public bool IsInRole(string roleName) =>
        Roles.Any(r => string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase));
}
