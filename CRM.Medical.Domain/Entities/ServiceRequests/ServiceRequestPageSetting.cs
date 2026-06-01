using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities.ServiceRequests;

public sealed class ServiceRequestPageSetting : BaseEntity
{
    public int Id { get; set; }

    public ServiceRequestPageType PageType { get; set; }

    public string AnnouncementTextAr { get; set; } = string.Empty;
    public string AnnouncementTextEn { get; set; } = string.Empty;

    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;

    public string DescriptionAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
