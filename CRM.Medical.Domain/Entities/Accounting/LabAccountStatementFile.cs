namespace CRM.Medical.Domain.Entities.Accounting;

public sealed class LabAccountStatementFile : BaseEntity
{
    public int Id { get; set; }

    public string LabClientId { get; set; } = string.Empty;
    public User LabClient { get; set; } = null!;

    public DateTime PeriodFrom { get; set; }

    public DateTime PeriodTo { get; set; }

    public string FileUrl { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
