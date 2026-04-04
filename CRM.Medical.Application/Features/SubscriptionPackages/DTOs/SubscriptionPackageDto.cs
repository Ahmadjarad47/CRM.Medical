using System.Text.Json;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.SubscriptionPackages.DTOs;

public sealed record SubscriptionPackageDto(
    int Id,
    string Name,
    decimal Price,
    int ValidityDays,
    JsonElement? IncludedTests,
    SubscriptionPackageTargetAudience TargetAudience,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
