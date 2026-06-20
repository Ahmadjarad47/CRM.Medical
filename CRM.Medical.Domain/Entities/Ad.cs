using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities;

public sealed class Ad : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public AdMediaType MediaType { get; set; }

    public string MediaUrl { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string AddressName { get; set; } = string.Empty;
}
