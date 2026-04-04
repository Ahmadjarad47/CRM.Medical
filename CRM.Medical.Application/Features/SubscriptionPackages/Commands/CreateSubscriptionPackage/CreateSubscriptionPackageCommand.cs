using System.Text.Json;
using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Commands.CreateSubscriptionPackage;

public sealed record CreateSubscriptionPackageCommand(
    string Name,
    decimal Price,
    int ValidityDays,
    JsonElement? IncludedTests,
    SubscriptionPackageTargetAudience TargetAudience,
    bool IsActive = true) : IRequest<SubscriptionPackageDto>;
