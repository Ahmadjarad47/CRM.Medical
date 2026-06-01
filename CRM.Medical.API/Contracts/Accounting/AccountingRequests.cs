using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Accounting;

public sealed record UpdateAccountingPageSettingRequest(
    string AnnouncementTextAr,
    string AnnouncementTextEn,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    bool IsActive);

public sealed record SaveLabAccountPaymentRequest(
    string LabClientId,
    decimal Amount,
    DateTime PaidAt,
    string PaymentMethod,
    string? ReferenceNumber,
    string? Notes);

public sealed class UploadLabAccountStatementPdfRequest
{
    public string LabClientId { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public IFormFile File { get; set; } = null!;
    public string? Notes { get; set; }
}
