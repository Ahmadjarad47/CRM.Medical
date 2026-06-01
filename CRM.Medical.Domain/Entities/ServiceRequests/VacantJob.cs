namespace CRM.Medical.Domain.Entities.ServiceRequests;

public sealed class VacantJob : BaseEntity
{
    public int Id { get; set; }

    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;

    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public ICollection<EmploymentApplicationRequest> EmploymentApplications { get; set; } = new List<EmploymentApplicationRequest>();
}
