using CRM.Medical.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.ServiceRequests;

public sealed record UpdateServiceRequestPageSettingRequest(
    string AnnouncementTextAr,
    string AnnouncementTextEn,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    bool IsActive);

public sealed record SaveVacantJobRequest(
    string TitleAr,
    string TitleEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive,
    int SortOrder);

public sealed class CreateEmploymentApplicationRequest
{
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
    public IFormFile CvFile { get; set; } = null!;
}

public sealed record UpdateRequestStatusRequest(string Status, string? Notes);

public sealed record CreateClientJoinRequest(
    string ManagerName,
    string LabName,
    string MobileNumber,
    string Email,
    string Address,
    string? AdditionalInfo);

public sealed record CreateContractServiceRequest(
    ContractRequestType ContractType,
    string ResponsibleName,
    string? OrganizationName,
    int ExpectedSubscribersCount,
    string ContactNumber,
    string Email,
    string Address,
    ContractDuration ContractDuration,
    string? AdditionalInfo);
