namespace CRM.Medical.Domain.Entities;

public sealed class CategoryMedical : BaseEntity
{
    public int Id { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<MedicalTest> MedicalTests { get; set; } = new List<MedicalTest>();
}
