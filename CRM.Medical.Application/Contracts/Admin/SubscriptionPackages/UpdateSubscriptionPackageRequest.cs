using System.Text.Json;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.API.Contracts.Admin.SubscriptionPackages;

public sealed record UpdateSubscriptionPackageRequest(
    string Name,
    decimal Price,
    int ValidityDays,
    JsonElement? IncludedTests,
    SubscriptionPackageTargetAudience TargetAudience,
    bool IsActive);
