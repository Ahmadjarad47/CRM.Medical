namespace CRM.Medical.Infrastructure.Seeding;

public sealed class DevelopmentSeedOptions
{
    public const string SectionName = "DevelopmentSeed";

    public bool Enabled { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;

    public List<SeedUserEntry> AdditionalUsers { get; init; } = [];
}

public sealed class SeedUserEntry
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Role { get; init; } = "Admin";
    public bool AllPermissions { get; init; } = true;
}
