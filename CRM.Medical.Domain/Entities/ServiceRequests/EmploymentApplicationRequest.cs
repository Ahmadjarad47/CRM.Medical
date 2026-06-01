namespace CRM.Medical.Domain.Entities.ServiceRequests;

public sealed class EmploymentApplicationRequest : BaseEntity
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string ResidencePlace { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AcademicDegree { get; set; } = string.Empty;

    public string PreviousExperience { get; set; } = string.Empty;

    public int YearsOfExperience { get; set; }

    public string Skills { get; set; } = string.Empty;

    public string? AdditionalCertificates { get; set; }

    public int VacantJobId { get; set; }

    public VacantJob VacantJob { get; set; } = null!;

    public string CvFileUrl { get; set; } = string.Empty;

    public string CvOriginalFileName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
