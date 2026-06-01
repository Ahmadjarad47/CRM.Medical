using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Accounting.DTOs;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Accounting.Services;

public interface IAccountingService
{
    Task<AccountingPageSettingDto> GetSettingsAsync(CancellationToken cancellationToken);
    Task<AccountingPageSettingDto> UpdateSettingsAsync(
        string announcementTextAr,
        string announcementTextEn,
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        bool isActive,
        CancellationToken cancellationToken);

    Task<LabAccountStatementDto> GetStatementAsync(
        string labClientId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken);

    Task<byte[]> GenerateStatementPdfAsync(
        string labClientId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken);

    Task<LabAccountStatementFileDto> UploadStatementPdfAsync(
        string labClientId,
        DateTime from,
        DateTime to,
        IFormFile file,
        string? notes,
        CancellationToken cancellationToken);

    Task<PagedResult<LabAccountPaymentDto>> ListPaymentsAsync(
        string? labClientId,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<LabAccountPaymentDto> CreatePaymentAsync(
        string labClientId,
        decimal amount,
        DateTime paidAt,
        string paymentMethod,
        string? referenceNumber,
        string? notes,
        CancellationToken cancellationToken);

    Task<LabAccountPaymentDto> UpdatePaymentAsync(
        int id,
        string labClientId,
        decimal amount,
        DateTime paidAt,
        string paymentMethod,
        string? referenceNumber,
        string? notes,
        CancellationToken cancellationToken);

    Task DeletePaymentAsync(int id, CancellationToken cancellationToken);
}
