using System.Net.Mail;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.ServiceRequests.DTOs;
using CRM.Medical.Application.Features.ServiceRequests.Services;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Entities.ServiceRequests;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.ServiceRequests;

public sealed class ServiceRequestServices(
    MedicalDbContext db,
    IFileStorageService fileStorage,
    IAccessPolicyEvaluator accessPolicyEvaluator)
    : IServiceRequestPageSettingsService,
        IVacantJobsService,
        IEmploymentApplicationService,
        IClientJoinRequestService,
        IContractServiceRequestService
{
    private const string EmploymentSuccessMessage = "تم استلام طلبك بنجاح. شكراً لك.";
    private const string MutawaliSuccessMessage = "تم استلام طلبك بنجاح. نشكر ثقتك بمخبر المتوالي.";

    private static readonly HashSet<string> EmploymentStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "New", "UnderReview", "Accepted", "Rejected"
    };

    private static readonly HashSet<string> RequestStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "New", "UnderReview", "Approved", "Rejected"
    };

    public async Task<ServiceRequestPageSettingDto> GetPublicByTypeAsync(
        ServiceRequestPageType pageType,
        CancellationToken cancellationToken)
    {
        var setting = await GetOrCreatePageSettingAsync(pageType, cancellationToken);
        if (!setting.IsActive)
            throw new ApplicationNotFoundException($"Service request page '{pageType}' was not found.");

        return MapPageSetting(setting);
    }

    public async Task<IReadOnlyList<ServiceRequestPageSettingDto>> ListAsync(CancellationToken cancellationToken)
    {
        await EnsureDefaultPageSettingsAsync(cancellationToken);
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.ServiceRequestPageSettings.AsNoTracking(),
            "service_request_page_settings",
            "read",
            cancellationToken);

        return await query.OrderBy(s => s.PageType)
            .Select(s => MapPageSetting(s))
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceRequestPageSettingDto> UpdateAsync(
        int id,
        string announcementTextAr,
        string announcementTextEn,
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var setting = await db.ServiceRequestPageSettings.FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Service request page setting '{id}' was not found.");

        await RequireAccessAsync(setting, "service_request_page_settings", "update", cancellationToken);
        setting.AnnouncementTextAr = Required(announcementTextAr, "Arabic announcement text");
        setting.AnnouncementTextEn = Required(announcementTextEn, "English announcement text");
        setting.TitleAr = Required(titleAr, "Arabic title");
        setting.TitleEn = Required(titleEn, "English title");
        setting.DescriptionAr = Required(descriptionAr, "Arabic description");
        setting.DescriptionEn = Required(descriptionEn, "English description");
        setting.IsActive = isActive;

        await db.SaveChangesAsync(cancellationToken);
        return MapPageSetting(setting);
    }

    public async Task<PagedResult<VacantJobDto>> ListAsync(
        int page,
        int pageSize,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        IQueryable<VacantJob> query = db.VacantJobs.AsNoTracking();
        query = includeInactive
            ? await accessPolicyEvaluator.ApplyAsync(query, "vacant_jobs", "read", cancellationToken)
            : query.Where(j => j.IsActive);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(j => j.SortOrder)
            .ThenByDescending(j => j.CreatedAt)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .Select(j => MapVacantJob(j))
            .ToListAsync(cancellationToken);

        return new PagedResult<VacantJobDto>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = total
        };
    }

    public async Task<VacantJobDto> GetByIdAsync(
        int id,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        IQueryable<VacantJob> query = db.VacantJobs.AsNoTracking();
        query = includeInactive
            ? await accessPolicyEvaluator.ApplyAsync(query, "vacant_jobs", "read", cancellationToken)
            : query.Where(j => j.IsActive);

        var job = await query.FirstOrDefaultAsync(j => j.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Vacant job '{id}' was not found.");
        return MapVacantJob(job);
    }

    public async Task<VacantJobDto> CreateAsync(
        string titleAr,
        string titleEn,
        string? descriptionAr,
        string? descriptionEn,
        bool isActive,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var entity = new VacantJob
        {
            TitleAr = Required(titleAr, "Arabic title"),
            TitleEn = Required(titleEn, "English title"),
            DescriptionAr = TrimToNull(descriptionAr),
            DescriptionEn = TrimToNull(descriptionEn),
            IsActive = isActive,
            SortOrder = sortOrder
        };

        await RequireAccessAsync(entity, "vacant_jobs", "create", cancellationToken);
        db.VacantJobs.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return MapVacantJob(entity);
    }

    public async Task<VacantJobDto> UpdateAsync(
        int id,
        string titleAr,
        string titleEn,
        string? descriptionAr,
        string? descriptionEn,
        bool isActive,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var entity = await db.VacantJobs.FirstOrDefaultAsync(j => j.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Vacant job '{id}' was not found.");
        await RequireAccessAsync(entity, "vacant_jobs", "update", cancellationToken);

        entity.TitleAr = Required(titleAr, "Arabic title");
        entity.TitleEn = Required(titleEn, "English title");
        entity.DescriptionAr = TrimToNull(descriptionAr);
        entity.DescriptionEn = TrimToNull(descriptionEn);
        entity.IsActive = isActive;
        entity.SortOrder = sortOrder;

        await db.SaveChangesAsync(cancellationToken);
        return MapVacantJob(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.VacantJobs.FirstOrDefaultAsync(j => j.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Vacant job '{id}' was not found.");
        await RequireAccessAsync(entity, "vacant_jobs", "delete", cancellationToken);

        var inUse = await db.EmploymentApplicationRequests.AnyAsync(r => r.VacantJobId == id, cancellationToken);
        if (inUse)
            entity.IsActive = false;
        else
            db.VacantJobs.Remove(entity);

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ServiceRequestSubmissionResponseDto> SubmitAsync(
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
        CancellationToken cancellationToken)
    {
        if (yearsOfExperience < 0)
            throw new ApplicationBadRequestException("Years of experience cannot be negative.");
        ValidateEmail(email);

        var jobExists = await db.VacantJobs.AnyAsync(j => j.Id == vacantJobId && j.IsActive, cancellationToken);
        if (!jobExists)
            throw new ApplicationBadRequestException($"Active vacant job '{vacantJobId}' was not found.");

        if (cvFile is null)
            throw new ApplicationBadRequestException("CV file is required.");

        var cvUrl = await fileStorage.UploadPdfAsync(cvFile, cancellationToken);
        db.EmploymentApplicationRequests.Add(new EmploymentApplicationRequest
        {
            FullName = Required(fullName, "Full name"),
            ResidencePlace = Required(residencePlace, "Residence place"),
            MobileNumber = Required(mobileNumber, "Mobile number"),
            Email = email.Trim(),
            AcademicDegree = Required(academicDegree, "Academic degree"),
            PreviousExperience = Required(previousExperience, "Previous experience"),
            YearsOfExperience = yearsOfExperience,
            Skills = Required(skills, "Skills"),
            AdditionalCertificates = TrimToNull(additionalCertificates),
            VacantJobId = vacantJobId,
            CvFileUrl = cvUrl,
            CvOriginalFileName = cvFile.FileName,
            Status = "New"
        });

        await db.SaveChangesAsync(cancellationToken);
        return new ServiceRequestSubmissionResponseDto(EmploymentSuccessMessage);
    }

    async Task<PagedResult<EmploymentApplicationRequestDto>> IEmploymentApplicationService.ListAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = db.EmploymentApplicationRequests.AsNoTracking().Include(r => r.VacantJob).AsQueryable();
        query = await accessPolicyEvaluator.ApplyAsync(query, "employment_application_requests", "read", cancellationToken);
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status.Trim());

        return await PageAsync(query, page, pageSize, MapEmploymentApplication, cancellationToken);
    }

    async Task<EmploymentApplicationRequestDto> IEmploymentApplicationService.GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.EmploymentApplicationRequests.AsNoTracking().Include(r => r.VacantJob),
            "employment_application_requests",
            "read",
            cancellationToken);
        var entity = await query.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Employment application '{id}' was not found.");
        return MapEmploymentApplication(entity);
    }

    async Task<EmploymentApplicationRequestDto> IEmploymentApplicationService.UpdateStatusAsync(
        int id,
        string status,
        string? notes,
        CancellationToken cancellationToken)
    {
        ValidateStatus(status, EmploymentStatuses);
        var entity = await db.EmploymentApplicationRequests.Include(r => r.VacantJob)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Employment application '{id}' was not found.");
        await RequireAccessAsync(entity, "employment_application_requests", "update", cancellationToken);

        entity.Status = status.Trim();
        entity.Notes = TrimToNull(notes);
        await db.SaveChangesAsync(cancellationToken);
        return MapEmploymentApplication(entity);
    }

    async Task IEmploymentApplicationService.DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.EmploymentApplicationRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Employment application '{id}' was not found.");
        await RequireAccessAsync(entity, "employment_application_requests", "delete", cancellationToken);

        db.EmploymentApplicationRequests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ServiceRequestSubmissionResponseDto> SubmitAsync(
        string managerName,
        string labName,
        string mobileNumber,
        string email,
        string address,
        string? additionalInfo,
        CancellationToken cancellationToken)
    {
        ValidateEmail(email);
        db.ClientJoinRequests.Add(new ClientJoinRequest
        {
            ManagerName = Required(managerName, "Manager name"),
            LabName = Required(labName, "Lab name"),
            MobileNumber = Required(mobileNumber, "Mobile number"),
            Email = email.Trim(),
            Address = Required(address, "Address"),
            AdditionalInfo = TrimToNull(additionalInfo),
            Status = "New"
        });

        await db.SaveChangesAsync(cancellationToken);
        return new ServiceRequestSubmissionResponseDto(MutawaliSuccessMessage);
    }

    async Task<PagedResult<ClientJoinRequestDto>> IClientJoinRequestService.ListAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.ClientJoinRequests.AsNoTracking(),
            "client_join_requests",
            "read",
            cancellationToken);
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status.Trim());

        return await PageAsync(query, page, pageSize, MapClientJoinRequest, cancellationToken);
    }

    async Task<ClientJoinRequestDto> IClientJoinRequestService.GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.ClientJoinRequests.AsNoTracking(),
            "client_join_requests",
            "read",
            cancellationToken);
        var entity = await query.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Client join request '{id}' was not found.");
        return MapClientJoinRequest(entity);
    }

    async Task<ClientJoinRequestDto> IClientJoinRequestService.UpdateStatusAsync(
        int id,
        string status,
        string? notes,
        CancellationToken cancellationToken)
    {
        ValidateStatus(status, RequestStatuses);
        var entity = await db.ClientJoinRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Client join request '{id}' was not found.");
        await RequireAccessAsync(entity, "client_join_requests", "update", cancellationToken);

        entity.Status = status.Trim();
        entity.Notes = TrimToNull(notes);
        await db.SaveChangesAsync(cancellationToken);
        return MapClientJoinRequest(entity);
    }

    async Task IClientJoinRequestService.DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.ClientJoinRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Client join request '{id}' was not found.");
        await RequireAccessAsync(entity, "client_join_requests", "delete", cancellationToken);

        db.ClientJoinRequests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ServiceRequestSubmissionResponseDto> SubmitAsync(
        ContractRequestType contractType,
        string responsibleName,
        string? organizationName,
        int expectedSubscribersCount,
        string contactNumber,
        string email,
        string address,
        ContractDuration contractDuration,
        string? additionalInfo,
        CancellationToken cancellationToken)
    {
        if (expectedSubscribersCount < 0)
            throw new ApplicationBadRequestException("Expected subscribers count cannot be negative.");
        if (contractType == ContractRequestType.Organization && string.IsNullOrWhiteSpace(organizationName))
            throw new ApplicationBadRequestException("Organization name is required for organization contracts.");
        ValidateEmail(email);

        db.ContractServiceRequests.Add(new ContractServiceRequest
        {
            ContractType = contractType,
            ResponsibleName = Required(responsibleName, "Responsible name"),
            OrganizationName = TrimToNull(organizationName),
            ExpectedSubscribersCount = expectedSubscribersCount,
            ContactNumber = Required(contactNumber, "Contact number"),
            Email = email.Trim(),
            Address = Required(address, "Address"),
            ContractDuration = contractDuration,
            AdditionalInfo = TrimToNull(additionalInfo),
            Status = "New"
        });

        await db.SaveChangesAsync(cancellationToken);
        return new ServiceRequestSubmissionResponseDto(MutawaliSuccessMessage);
    }

    async Task<PagedResult<ContractServiceRequestDto>> IContractServiceRequestService.ListAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.ContractServiceRequests.AsNoTracking(),
            "contract_service_requests",
            "read",
            cancellationToken);
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status.Trim());

        return await PageAsync(query, page, pageSize, MapContractServiceRequest, cancellationToken);
    }

    async Task<ContractServiceRequestDto> IContractServiceRequestService.GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.ContractServiceRequests.AsNoTracking(),
            "contract_service_requests",
            "read",
            cancellationToken);
        var entity = await query.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Contract service request '{id}' was not found.");
        return MapContractServiceRequest(entity);
    }

    async Task<ContractServiceRequestDto> IContractServiceRequestService.UpdateStatusAsync(
        int id,
        string status,
        string? notes,
        CancellationToken cancellationToken)
    {
        ValidateStatus(status, RequestStatuses);
        var entity = await db.ContractServiceRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Contract service request '{id}' was not found.");
        await RequireAccessAsync(entity, "contract_service_requests", "update", cancellationToken);

        entity.Status = status.Trim();
        entity.Notes = TrimToNull(notes);
        await db.SaveChangesAsync(cancellationToken);
        return MapContractServiceRequest(entity);
    }

    async Task IContractServiceRequestService.DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.ContractServiceRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Contract service request '{id}' was not found.");
        await RequireAccessAsync(entity, "contract_service_requests", "delete", cancellationToken);

        db.ContractServiceRequests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<ServiceRequestPageSetting> GetOrCreatePageSettingAsync(
        ServiceRequestPageType pageType,
        CancellationToken cancellationToken)
    {
        var setting = await db.ServiceRequestPageSettings.FirstOrDefaultAsync(s => s.PageType == pageType, cancellationToken);
        if (setting is not null)
            return setting;

        setting = new ServiceRequestPageSetting
        {
            PageType = pageType,
            AnnouncementTextAr = string.Empty,
            AnnouncementTextEn = string.Empty,
            TitleAr = string.Empty,
            TitleEn = string.Empty,
            DescriptionAr = string.Empty,
            DescriptionEn = string.Empty,
            IsActive = true
        };
        db.ServiceRequestPageSettings.Add(setting);
        await db.SaveChangesAsync(cancellationToken);
        return setting;
    }

    private async Task EnsureDefaultPageSettingsAsync(CancellationToken cancellationToken)
    {
        foreach (var pageType in Enum.GetValues<ServiceRequestPageType>())
            await GetOrCreatePageSettingAsync(pageType, cancellationToken);
    }

    private async Task RequireAccessAsync<TEntity>(
        TEntity entity,
        string resource,
        string action,
        CancellationToken cancellationToken)
    {
        if (!await accessPolicyEvaluator.CanAccessAsync(entity, resource, action, cancellationToken))
            throw new ApplicationForbiddenException($"You cannot {action} this resource.");
    }

    private static async Task<PagedResult<TDto>> PageAsync<TEntity, TDto>(
        IQueryable<TEntity> query,
        int page,
        int pageSize,
        Func<TEntity, TDto> mapper,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var total = await query.CountAsync(cancellationToken);
        var entities = await query
            .OrderByDescending(e => EF.Property<DateTime>(e!, nameof(BaseEntity.CreatedAt)))
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TDto>
        {
            Items = entities.Select(mapper).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = total
        };
    }

    private static string Required(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ApplicationBadRequestException($"{fieldName} is required.");
        return value.Trim();
    }

    private static string? TrimToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ApplicationBadRequestException("Email is required.");
        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new ApplicationBadRequestException("Email is invalid.");
        }
    }

    private static void ValidateStatus(string status, HashSet<string> allowed)
    {
        if (string.IsNullOrWhiteSpace(status) || !allowed.Contains(status.Trim()))
            throw new ApplicationBadRequestException("Status is invalid.");
    }

    private static ServiceRequestPageSettingDto MapPageSetting(ServiceRequestPageSetting setting) =>
        new(
            setting.Id,
            setting.PageType,
            setting.AnnouncementTextAr,
            setting.AnnouncementTextEn,
            setting.TitleAr,
            setting.TitleEn,
            setting.DescriptionAr,
            setting.DescriptionEn,
            setting.IsActive,
            setting.CreatedAt,
            setting.UpdatedAt);

    private static VacantJobDto MapVacantJob(VacantJob job) =>
        new(
            job.Id,
            job.TitleAr,
            job.TitleEn,
            job.DescriptionAr,
            job.DescriptionEn,
            job.IsActive,
            job.SortOrder,
            job.CreatedAt,
            job.UpdatedAt);

    private static EmploymentApplicationRequestDto MapEmploymentApplication(EmploymentApplicationRequest request) =>
        new(
            request.Id,
            request.FullName,
            request.ResidencePlace,
            request.MobileNumber,
            request.Email,
            request.AcademicDegree,
            request.PreviousExperience,
            request.YearsOfExperience,
            request.Skills,
            request.AdditionalCertificates,
            request.VacantJobId,
            request.VacantJob.TitleAr,
            request.VacantJob.TitleEn,
            request.CvFileUrl,
            request.CvOriginalFileName,
            request.Status,
            request.Notes,
            request.CreatedAt,
            request.UpdatedAt);

    private static ClientJoinRequestDto MapClientJoinRequest(ClientJoinRequest request) =>
        new(
            request.Id,
            request.ManagerName,
            request.LabName,
            request.MobileNumber,
            request.Email,
            request.Address,
            request.AdditionalInfo,
            request.Status,
            request.Notes,
            request.CreatedAt,
            request.UpdatedAt);

    private static ContractServiceRequestDto MapContractServiceRequest(ContractServiceRequest request) =>
        new(
            request.Id,
            request.ContractType,
            request.ResponsibleName,
            request.OrganizationName,
            request.ExpectedSubscribersCount,
            request.ContactNumber,
            request.Email,
            request.Address,
            request.ContractDuration,
            request.AdditionalInfo,
            request.Status,
            request.Notes,
            request.CreatedAt,
            request.UpdatedAt);
}
