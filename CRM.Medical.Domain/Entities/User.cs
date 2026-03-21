using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Domain.Entities;

/// <summary>
/// Application user stored in the <c>Users</c> table; extends ASP.NET Core Identity.
/// </summary>
public sealed class User : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}

