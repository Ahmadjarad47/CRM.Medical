using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Insurance.DTOs;
using CRM.Medical.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Insurance.Services;

public interface IInsuranceApprovalRequestService
{
    Task<InsuranceApprovalSubmissionResponseDto> SubmitAsync(
        string patientId,
        string insuredName,
        string insuranceNumber,
        string mobileNumber,
        IFormFile insuranceCardImage,
        IFormFile prescriptionImage,
        CancellationToken cancellationToken);

    Task<PagedResult<InsuranceApprovalRequestDto>> ListMyAsync(
        string patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<InsuranceApprovalRequestDetailsDto> GetMyByIdAsync(
        string patientId,
        int id,
        CancellationToken cancellationToken);

    Task<PagedResult<InsuranceApprovalRequestDto>> ListAsync(
        int page,
        int pageSize,
        InsuranceApprovalRequestStatus? status,
        string? search,
        CancellationToken cancellationToken);

    Task<InsuranceApprovalRequestDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<InsuranceApprovalRequestDetailsDto> UpdateStatusAsync(
        int id,
        InsuranceApprovalRequestStatus status,
        string? notes,
        string? rejectionReason,
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
