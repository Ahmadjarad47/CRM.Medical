using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Domain.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? City { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Flexible JSONB column for role-specific profile data.
    /// Example: { "doctor": { "specialization": "cardiology" }, "patient": { "bloodType": "O+" } }
    /// </summary>
    public JsonDocument? ProfileMetadata { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
