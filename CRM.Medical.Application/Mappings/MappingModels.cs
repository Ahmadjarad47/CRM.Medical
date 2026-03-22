namespace CRM.Medical.Application.Mappings;

internal sealed record UserRolesMappingModel(string UserId, IReadOnlyList<string> Roles);

internal sealed record UserPermissionsMappingModel(string UserId, IReadOnlyList<string> Permissions);

internal sealed record LoginResponseMappingModel(
    string UserId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    string Email,
    string DisplayName);
