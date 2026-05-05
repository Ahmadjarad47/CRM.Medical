namespace CRM.Medical.Application.Authorization;

public readonly record struct PermissionDescriptor(string Resource, string Action)
{
    public string Key => $"{Resource}:{Action}";

    public static PermissionDescriptor FromPolicyName(string policyName)
    {
        var parts = policyName.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            throw new ArgumentException("Permission policy must have the format 'Resource:Action'.", nameof(policyName));

        return new PermissionDescriptor(parts[0], parts[1]);
    }
}
