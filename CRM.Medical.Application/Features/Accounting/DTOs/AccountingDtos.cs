namespace CRM.Medical.Application.Features.Accounting.DTOs;

public sealed record AccountingPageSettingDto(
    int Id,
    string AnnouncementTextAr,
    string AnnouncementTextEn,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record LabAccountPaymentDto(
    int Id,
    string LabClientId,
    string LabName,
    decimal Amount,
    DateTime PaidAt,
    string PaymentMethod,
    string? ReferenceNumber,
    string? Notes,
    DateTime CreatedAt);

public sealed record LabAccountStatementFileDto(
    int Id,
    string LabClientId,
    string LabName,
    DateTime PeriodFrom,
    DateTime PeriodTo,
    string FileUrl,
    string OriginalFileName,
    string? Notes,
    DateTime CreatedAt);

public sealed record LabAccountStatementRowDto(
    DateTime RequestDate,
    int TestRequestId,
    string PatientName,
    string TestNameAr,
    string TestNameEn,
    decimal TestPrice,
    decimal PaymentsApplied);

public sealed record LabAccountStatementTotalsDto(
    decimal TotalTestsAmount,
    decimal TotalPayments,
    decimal RemainingAmount,
    decimal PreviousBalance,
    decimal BalanceUntilPeriodEnd);

public sealed record AccountingChartPointDto(string Label, decimal TestsAmount, decimal PaymentsAmount, decimal RemainingAmount);

public sealed record AccountingAnalysisDto(
    int TestsCount,
    int DistinctPatientsCount,
    decimal AverageTestPrice,
    decimal PaymentCoveragePercentage,
    string HighestRevenueTestName,
    decimal HighestRevenueTestAmount);

public sealed record LabAccountStatementDto(
    AccountingPageSettingDto Settings,
    string LabClientId,
    string LabName,
    DateTime PeriodFrom,
    DateTime PeriodTo,
    decimal LabOutstandingBalance,
    IReadOnlyList<LabAccountStatementRowDto> Rows,
    LabAccountStatementTotalsDto Totals,
    IReadOnlyList<AccountingChartPointDto> Chart,
    AccountingAnalysisDto Analysis,
    IReadOnlyList<LabAccountStatementFileDto> UploadedFiles);
