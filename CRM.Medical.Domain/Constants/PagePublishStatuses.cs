namespace CRM.Medical.Domain.Constants;

/// <summary>Publish lifecycle values for <see cref="Entities.Page"/>.</summary>
public static class PagePublishStatuses
{
    public const string Draft = "Draft";
    public const string Scheduled = "Scheduled";
    public const string Published = "Published";
    public const string Archived = "Archived";

    public static readonly IReadOnlyList<string> All = [Draft, Scheduled, Published, Archived];
}
