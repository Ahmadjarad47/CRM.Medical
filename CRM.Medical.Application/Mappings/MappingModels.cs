namespace CRM.Medical.Application.Mappings;

internal sealed record UserRolesMappingModel(string UserId, IReadOnlyList<string> Roles);

internal sealed record UserPermissionsMappingModel(string UserId, IReadOnlyList<string> Permissions);

internal sealed record LoginResponseMappingModel(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    string? Email,
    string DisplayName);
