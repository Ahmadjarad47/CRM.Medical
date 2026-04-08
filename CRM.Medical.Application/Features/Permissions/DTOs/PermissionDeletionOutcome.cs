namespace CRM.Medical.Application.Features.Permissions.DTOs;

public sealed record PermissionDeletionOutcome(IReadOnlyList<string> UserIdsForCacheInvalidation);
