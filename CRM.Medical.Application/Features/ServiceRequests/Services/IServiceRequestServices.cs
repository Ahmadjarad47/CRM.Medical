using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ServiceRequests.DTOs;
using CRM.Medical.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.ServiceRequests.Services;

public interface IServiceRequestPageSettingsService
{
    Task<ServiceRequestPageSettingDto> GetPublicByTypeAsync(ServiceRequestPageType pageType, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceRequestPageSettingDto>> ListAsync(CancellationToken cancellationToken);
    Task<ServiceRequestPageSettingDto> UpdateAsync(
        int id,
        string announcementTextAr,
        string announcementTextEn,
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        bool isActive,
        CancellationToken cancellationToken);
}

public interface IVacantJobsService
{
    Task<PagedResult<VacantJobDto>> ListAsync(int page, int pageSize, bool includeInactive, CancellationToken cancellationToken);
    Task<VacantJobDto> GetByIdAsync(int id, bool includeInactive, CancellationToken cancellationToken);
    Task<VacantJobDto> CreateAsync(
        string titleAr,
        string titleEn,
        string? descriptionAr,
        string? descriptionEn,
        bool isActive,
        int sortOrder,
        CancellationToken cancellationToken);
    Task<VacantJobDto> UpdateAsync(
        int id,
        string titleAr,
        string titleEn,
        string? descriptionAr,
        string? descriptionEn,
        bool isActive,
        int sortOrder,
        CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

public interface IEmploymentApplicationService
{
    Task<ServiceRequestSubmissionResponseDto> SubmitAsync(
        string fullName,
        string residencePlace,
        string mobileNumber,
        string email,
        string academicDegree,
        string previousExperience,
        int yearsOfExperience,
        string skills,
        string? additionalCertificates,
        int vacantJobId,
        IFormFile cvFile,
        CancellationToken cancellationToken);

    Task<PagedResult<EmploymentApplicationRequestDto>> ListAsync(int page, int pageSize, string? status, CancellationToken cancellationToken);
    Task<EmploymentApplicationRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<EmploymentApplicationRequestDto> UpdateStatusAsync(int id, string status, string? notes, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

public interface IClientJoinRequestService
{
    Task<ServiceRequestSubmissionResponseDto> SubmitAsync(
        string managerName,
        string labName,
        string mobileNumber,
        string email,
        string address,
        string? additionalInfo,
        CancellationToken cancellationToken);

    Task<PagedResult<ClientJoinRequestDto>> ListAsync(int page, int pageSize, string? status, CancellationToken cancellationToken);
    Task<ClientJoinRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ClientJoinRequestDto> UpdateStatusAsync(int id, string status, string? notes, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

public interface IContractServiceRequestService
{
    Task<ServiceRequestSubmissionResponseDto> SubmitAsync(
        ContractRequestType contractType,
        string responsibleName,
        string? organizationName,
        int expectedSubscribersCount,
        string contactNumber,
        string email,
        string address,
        ContractDuration contractDuration,
        string? additionalInfo,
        CancellationToken cancellationToken);

    Task<PagedResult<ContractServiceRequestDto>> ListAsync(int page, int pageSize, string? status, CancellationToken cancellationToken);
    Task<ContractServiceRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ContractServiceRequestDto> UpdateStatusAsync(int id, string status, string? notes, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
