using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities;

public sealed class WelcomePage : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public AdMediaType MediaType { get; set; }

    public string MediaUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
