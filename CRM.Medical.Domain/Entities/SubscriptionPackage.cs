using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class SubscriptionPackage
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int ValidityDays { get; set; }

    /// <summary>JSON payload describing bundled tests or services (e.g. array of test codes).</summary>
    public JsonDocument? IncludedTests { get; set; }

    public SubscriptionPackageTargetAudience TargetAudience { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
