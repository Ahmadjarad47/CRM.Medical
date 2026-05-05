namespace CRM.Medical.Application.Authorization;

public sealed class PolicyEvaluationContext
{
    public required string UserId { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }

    public required PermissionDescriptor Permission { get; init; }

    public IReadOnlyDictionary<string, object?> Resource { get; init; } =
        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, object?> Request { get; init; } =
        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
