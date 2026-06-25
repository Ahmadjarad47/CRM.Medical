namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class SaveCategoryMedicalRequest
{
    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
