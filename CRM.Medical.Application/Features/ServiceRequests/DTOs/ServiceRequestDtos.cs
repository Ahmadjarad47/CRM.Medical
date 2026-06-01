using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.ServiceRequests.DTOs;

public sealed record ServiceRequestPageSettingDto(
    int Id,
    ServiceRequestPageType PageType,
    string AnnouncementTextAr,
    string AnnouncementTextEn,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record VacantJobDto(
    int Id,
    string TitleAr,
    string TitleEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record EmploymentApplicationRequestDto(
    int Id,
    string FullName,
    string ResidencePlace,
    string MobileNumber,
    string Email,
    string AcademicDegree,
    string PreviousExperience,
    int YearsOfExperience,
    string Skills,
    string? AdditionalCertificates,
    int VacantJobId,
    string VacantJobTitleAr,
    string VacantJobTitleEn,
    string CvFileUrl,
    string CvOriginalFileName,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ClientJoinRequestDto(
    int Id,
    string ManagerName,
    string LabName,
    string MobileNumber,
    string Email,
    string Address,
    string? AdditionalInfo,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ContractServiceRequestDto(
    int Id,
    ContractRequestType ContractType,
    string ResponsibleName,
    string? OrganizationName,
    int ExpectedSubscribersCount,
    string ContactNumber,
    string Email,
    string Address,
    ContractDuration ContractDuration,
    string? AdditionalInfo,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ServiceRequestSubmissionResponseDto(string Message);
