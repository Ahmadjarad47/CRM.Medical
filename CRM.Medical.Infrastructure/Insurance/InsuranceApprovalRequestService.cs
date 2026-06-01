using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Insurance.DTOs;
using CRM.Medical.Application.Features.Insurance.Services;
using CRM.Medical.Application.Features.Notifications.Services;
using CRM.Medical.Domain.Entities.Insurance;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Insurance;

public sealed class InsuranceApprovalRequestService(
    MedicalDbContext db,
    IFileStorageService fileStorage,
    IAccessPolicyEvaluator accessPolicyEvaluator,
    INotificationService notifications,
    ILogger<InsuranceApprovalRequestService> logger) : IInsuranceApprovalRequestService
{
    private const string SuccessMessage =
        "تم استلام الطلب وسيتم إرسال إشعار إليك لتتبع حالة الطلب. شكراً لثقتك بمخبر المتوالي للتحاليل الطبية.";

    public async Task<InsuranceApprovalSubmissionResponseDto> SubmitAsync(
        string patientId,
        string insuredName,
        string insuranceNumber,
        string mobileNumber,
        IFormFile insuranceCardImage,
        IFormFile prescriptionImage,
        CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(patientId, cancellationToken);

        if (insuranceCardImage is null)
            throw new ApplicationBadRequestException("Insurance card image is required.");
        if (prescriptionImage is null)
            throw new ApplicationBadRequestException("Prescription image is required.");

        var cardUrl = await fileStorage.UploadImageAsync(insuranceCardImage, cancellationToken);
        var prescriptionUrl = await fileStorage.UploadImageAsync(prescriptionImage, cancellationToken);

        db.InsuranceApprovalRequests.Add(new InsuranceApprovalRequest
        {
            PatientId = patientId,
            InsuredName = Required(insuredName, "Insured name"),
            InsuranceNumber = Required(insuranceNumber, "Insurance number"),
            MobileNumber = Required(mobileNumber, "Mobile number"),
            InsuranceCardImageUrl = cardUrl,
            InsuranceCardOriginalFileName = insuranceCardImage.FileName,
            PrescriptionImageUrl = prescriptionUrl,
            PrescriptionOriginalFileName = prescriptionImage.FileName,
            Status = InsuranceApprovalRequestStatus.New
        });

        await db.SaveChangesAsync(cancellationToken);
        return new InsuranceApprovalSubmissionResponseDto(SuccessMessage);
    }

    public async Task<PagedResult<InsuranceApprovalRequestDto>> ListMyAsync(
        string patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = db.InsuranceApprovalRequests
            .AsNoTracking()
            .Include(r => r.Patient)
            .Where(r => r.PatientId == patientId);

        return await PageAsync(query, page, pageSize, MapSummary, cancellationToken);
    }

    public async Task<InsuranceApprovalRequestDetailsDto> GetMyByIdAsync(
        string patientId,
        int id,
        CancellationToken cancellationToken)
    {
        var entity = await db.InsuranceApprovalRequests
            .AsNoTracking()
            .Include(r => r.Patient)
            .FirstOrDefaultAsync(r => r.Id == id && r.PatientId == patientId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Insurance approval request '{id}' was not found.");

        return MapDetails(entity);
    }

    public async Task<PagedResult<InsuranceApprovalRequestDto>> ListAsync(
        int page,
        int pageSize,
        InsuranceApprovalRequestStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = db.InsuranceApprovalRequests
            .AsNoTracking()
            .Include(r => r.Patient)
            .AsQueryable();

        query = await accessPolicyEvaluator.ApplyAsync(query, "insurance_approval_requests", "read", cancellationToken);

        if (status is not null)
            query = query.Where(r => r.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(r =>
                r.InsuredName.ToLower().Contains(term) ||
                r.InsuranceNumber.ToLower().Contains(term) ||
                r.MobileNumber.ToLower().Contains(term) ||
                r.Patient.FullName.ToLower().Contains(term));
        }

        return await PageAsync(query, page, pageSize, MapSummary, cancellationToken);
    }

    public async Task<InsuranceApprovalRequestDetailsDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var query = db.InsuranceApprovalRequests
            .AsNoTracking()
            .Include(r => r.Patient)
            .AsQueryable();

        query = await accessPolicyEvaluator.ApplyAsync(query, "insurance_approval_requests", "read", cancellationToken);

        var entity = await query.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Insurance approval request '{id}' was not found.");

        return MapDetails(entity);
    }

    public async Task<InsuranceApprovalRequestDetailsDto> UpdateStatusAsync(
        int id,
        InsuranceApprovalRequestStatus status,
        string? notes,
        string? rejectionReason,
        CancellationToken cancellationToken)
    {
        var entity = await db.InsuranceApprovalRequests
            .Include(r => r.Patient)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Insurance approval request '{id}' was not found.");

        await RequireAccessAsync(entity, "update", cancellationToken);
        entity.Status = status;
        entity.Notes = TrimToNull(notes);
        entity.RejectionReason = status == InsuranceApprovalRequestStatus.Rejected
            ? TrimToNull(rejectionReason)
            : null;

        await db.SaveChangesAsync(cancellationToken);
        await NotifyStatusChangedAsync(entity, cancellationToken);

        return MapDetails(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.InsuranceApprovalRequests
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Insurance approval request '{id}' was not found.");

        await RequireAccessAsync(entity, "delete", cancellationToken);
        db.InsuranceApprovalRequests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUserExistsAsync(string patientId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(patientId))
            throw new ApplicationUnauthorizedException("Unable to identify the current patient.");

        var exists = await db.Users.AnyAsync(u => u.Id == patientId, cancellationToken);
        if (!exists)
            throw new ApplicationNotFoundException($"Patient '{patientId}' was not found.");
    }

    private async Task RequireAccessAsync(
        InsuranceApprovalRequest request,
        string action,
        CancellationToken cancellationToken)
    {
        if (!await accessPolicyEvaluator.CanAccessAsync(request, "insurance_approval_requests", action, cancellationToken))
            throw new ApplicationForbiddenException($"You cannot {action} this insurance approval request.");
    }

    private async Task NotifyStatusChangedAsync(
        InsuranceApprovalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await notifications.SendToUserAsync(
                request.PatientId,
                "Insurance approval request updated",
                $"Your insurance approval request status is now {request.Status}.",
                new Dictionary<string, string>
                {
                    ["type"] = "insurance_approval_request",
                    ["requestId"] = request.Id.ToString(),
                    ["status"] = request.Status.ToString()
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to notify patient {PatientId} about insurance approval request {RequestId}.", request.PatientId, request.Id);
        }
    }

    private static async Task<PagedResult<TDto>> PageAsync<TDto>(
        IQueryable<InsuranceApprovalRequest> query,
        int page,
        int pageSize,
        Func<InsuranceApprovalRequest, TDto> mapper,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var total = await query.CountAsync(cancellationToken);
        var entities = await query
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id)
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

    private static InsuranceApprovalRequestDto MapSummary(InsuranceApprovalRequest request) =>
        new(
            request.Id,
            request.PatientId,
            request.Patient.FullName,
            request.InsuredName,
            request.InsuranceNumber,
            request.MobileNumber,
            request.Status,
            request.Notes,
            request.RejectionReason,
            request.CreatedAt,
            request.UpdatedAt);

    private static InsuranceApprovalRequestDetailsDto MapDetails(InsuranceApprovalRequest request) =>
        new(
            request.Id,
            request.PatientId,
            request.Patient.FullName,
            request.InsuredName,
            request.InsuranceNumber,
            request.MobileNumber,
            request.InsuranceCardImageUrl,
            request.InsuranceCardOriginalFileName,
            request.PrescriptionImageUrl,
            request.PrescriptionOriginalFileName,
            request.Status,
            request.Notes,
            request.RejectionReason,
            request.CreatedAt,
            request.UpdatedAt);
}
