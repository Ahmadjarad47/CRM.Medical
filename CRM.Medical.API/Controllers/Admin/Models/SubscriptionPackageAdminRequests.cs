using System.Text.Json;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.API.Controllers.Admin.Models;

public sealed record ListSubscriptionPackagesRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public SubscriptionPackageTargetAudience? TargetAudience { get; init; }
    public bool? IsActive { get; init; }
}

public sealed record CreateSubscriptionPackageRequest(
    string Name,
    decimal Price,
    int ValidityDays,
    JsonElement? IncludedTests,
    SubscriptionPackageTargetAudience TargetAudience,
    bool IsActive = true);

public sealed record UpdateSubscriptionPackageRequest(
    string Name,
    decimal Price,
    int ValidityDays,
    JsonElement? IncludedTests,
    SubscriptionPackageTargetAudience TargetAudience,
    bool IsActive);
