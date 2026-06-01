using System.Globalization;
using System.Text;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Accounting.DTOs;
using CRM.Medical.Application.Features.Accounting.Services;
using CRM.Medical.Domain.Entities.Accounting;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Accounting;

public sealed class AccountingService(
    MedicalDbContext db,
    IFileStorageService fileStorage,
    IAccessPolicyEvaluator accessPolicyEvaluator) : IAccountingService
{
    public async Task<AccountingPageSettingDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var settings = await GetOrCreateSettingsAsync(cancellationToken);
        await RequireAccessAsync(settings, "accounting_page_settings", "read", cancellationToken);
        return MapSettings(settings);
    }

    public async Task<AccountingPageSettingDto> UpdateSettingsAsync(
        string announcementTextAr,
        string announcementTextEn,
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var settings = await GetOrCreateSettingsAsync(cancellationToken);
        await RequireAccessAsync(settings, "accounting_page_settings", "update", cancellationToken);
        settings.AnnouncementTextAr = announcementTextAr.Trim();
        settings.AnnouncementTextEn = announcementTextEn.Trim();
        settings.TitleAr = titleAr.Trim();
        settings.TitleEn = titleEn.Trim();
        settings.DescriptionAr = descriptionAr.Trim();
        settings.DescriptionEn = descriptionEn.Trim();
        settings.IsActive = isActive;

        await db.SaveChangesAsync(cancellationToken);
        return MapSettings(settings);
    }

    public async Task<LabAccountStatementDto> GetStatementAsync(
        string labClientId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken)
    {
        var period = NormalizePeriod(from, to);
        var settings = await GetOrCreateSettingsAsync(cancellationToken);
        var lab = await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == labClientId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Lab client '{labClientId}' was not found.");

        var testRequestsQuery = db.TestRequests
            .AsNoTracking()
            .Include(r => r.MedicalTest)
            .Include(r => r.ExternalPatient)
            .Where(r => r.LabClientId == labClientId &&
                        r.RequestDate >= period.FromInclusive &&
                        r.RequestDate < period.ToExclusive)
            .AsQueryable();
        testRequestsQuery = await accessPolicyEvaluator.ApplyAsync(testRequestsQuery, "test_requests", "read", cancellationToken);
        var testRequests = await testRequestsQuery.OrderBy(r => r.RequestDate).ThenBy(r => r.Id).ToListAsync(cancellationToken);

        var directPatientIds = testRequests
            .Select(r => r.DirectPatientId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToArray();

        var directPatientNames = await db.Users.AsNoTracking()
            .Where(u => directPatientIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        var periodPaymentsQuery = db.LabAccountPayments.AsNoTracking()
            .Where(p => p.LabClientId == labClientId &&
                        p.PaidAt >= period.FromInclusive &&
                        p.PaidAt < period.ToExclusive)
            .AsQueryable();
        periodPaymentsQuery = await accessPolicyEvaluator.ApplyAsync(periodPaymentsQuery, "lab_account_payments", "read", cancellationToken);
        var periodPayments = await periodPaymentsQuery.OrderBy(p => p.PaidAt).ThenBy(p => p.Id).ToListAsync(cancellationToken);

        var totalPayments = periodPayments.Sum(p => p.Amount);
        var remainingPaymentPool = totalPayments;
        var rows = new List<LabAccountStatementRowDto>();
        foreach (var request in testRequests)
        {
            var price = Convert.ToDecimal(request.TotalAmount);
            if (price <= 0)
                price = Convert.ToDecimal(request.MedicalTest.Price);

            var appliedPayment = Math.Min(price, remainingPaymentPool);
            remainingPaymentPool -= appliedPayment;

            rows.Add(new LabAccountStatementRowDto(
                request.RequestDate,
                request.Id,
                ResolvePatientName(request.DirectPatientId, request.ExternalPatient?.FullName, directPatientNames),
                request.MedicalTest.NameAr,
                request.MedicalTest.NameEn,
                price,
                appliedPayment));
        }

        var totalTests = rows.Sum(r => r.TestPrice);
        var previousBalance = await CalculateBalanceBeforeAsync(labClientId, period.FromInclusive, cancellationToken);
        var balanceUntilEnd = await CalculateBalanceBeforeAsync(labClientId, period.ToExclusive, cancellationToken);
        var fileEntities = await db.LabAccountStatementFiles.AsNoTracking()
            .Include(f => f.LabClient)
            .Where(f => f.LabClientId == labClientId &&
                        f.PeriodFrom == period.FromInclusive &&
                        f.PeriodTo == period.ToInclusive)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
        var files = fileEntities.Select(MapStatementFile).ToList();

        return new LabAccountStatementDto(
            MapSettings(settings),
            labClientId,
            lab.FullName,
            period.FromInclusive,
            period.ToInclusive,
            balanceUntilEnd,
            rows,
            new LabAccountStatementTotalsDto(
                totalTests,
                totalPayments,
                totalTests - totalPayments,
                previousBalance,
                balanceUntilEnd),
            BuildChart(rows, periodPayments, period.FromInclusive, period.ToInclusive),
            BuildAnalysis(rows),
            files);
    }

    public async Task<byte[]> GenerateStatementPdfAsync(
        string labClientId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken)
    {
        var statement = await GetStatementAsync(labClientId, from, to, cancellationToken);
        return SimplePdf.Create(BuildStatementPdfLines(statement));
    }

    public async Task<LabAccountStatementFileDto> UploadStatementPdfAsync(
        string labClientId,
        DateTime from,
        DateTime to,
        IFormFile file,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (file is null)
            throw new ApplicationBadRequestException("PDF file is required.");

        var period = NormalizePeriod(from, to);
        var lab = await db.Users.FirstOrDefaultAsync(u => u.Id == labClientId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Lab client '{labClientId}' was not found.");
        var url = await fileStorage.UploadPdfAsync(file, cancellationToken);

        var entity = new LabAccountStatementFile
        {
            LabClientId = labClientId,
            LabClient = lab,
            PeriodFrom = period.FromInclusive,
            PeriodTo = period.ToInclusive,
            FileUrl = url,
            OriginalFileName = file.FileName,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        await RequireAccessAsync(entity, "lab_account_statement_files", "create", cancellationToken);
        db.LabAccountStatementFiles.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return MapStatementFile(entity);
    }

    public async Task<PagedResult<LabAccountPaymentDto>> ListPaymentsAsync(
        string? labClientId,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = db.LabAccountPayments.AsNoTracking().Include(p => p.LabClient).AsQueryable();
        query = await accessPolicyEvaluator.ApplyAsync(query, "lab_account_payments", "read", cancellationToken);
        if (!string.IsNullOrWhiteSpace(labClientId))
            query = query.Where(p => p.LabClientId == labClientId);
        if (from is not null)
            query = query.Where(p => p.PaidAt >= from.Value.Date);
        if (to is not null)
            query = query.Where(p => p.PaidAt < to.Value.Date.AddDays(1));

        var total = await query.CountAsync(cancellationToken);
        var paymentEntities = await query
            .OrderByDescending(p => p.PaidAt)
            .ThenByDescending(p => p.Id)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);
        var items = paymentEntities.Select(MapPayment).ToList();

        return new PagedResult<LabAccountPaymentDto>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = total
        };
    }

    public async Task<LabAccountPaymentDto> CreatePaymentAsync(
        string labClientId,
        decimal amount,
        DateTime paidAt,
        string paymentMethod,
        string? referenceNumber,
        string? notes,
        CancellationToken cancellationToken)
    {
        ValidatePayment(amount, paymentMethod);
        var lab = await db.Users.FirstOrDefaultAsync(u => u.Id == labClientId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Lab client '{labClientId}' was not found.");

        var entity = new LabAccountPayment
        {
            LabClientId = labClientId,
            LabClient = lab,
            Amount = amount,
            PaidAt = paidAt,
            PaymentMethod = paymentMethod.Trim(),
            ReferenceNumber = TrimToNull(referenceNumber),
            Notes = TrimToNull(notes)
        };

        await RequireAccessAsync(entity, "lab_account_payments", "create", cancellationToken);
        db.LabAccountPayments.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return MapPayment(entity);
    }

    public async Task<LabAccountPaymentDto> UpdatePaymentAsync(
        int id,
        string labClientId,
        decimal amount,
        DateTime paidAt,
        string paymentMethod,
        string? referenceNumber,
        string? notes,
        CancellationToken cancellationToken)
    {
        ValidatePayment(amount, paymentMethod);
        var entity = await db.LabAccountPayments.Include(p => p.LabClient)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Payment '{id}' was not found.");
        await RequireAccessAsync(entity, "lab_account_payments", "update", cancellationToken);
        var lab = await db.Users.FirstOrDefaultAsync(u => u.Id == labClientId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Lab client '{labClientId}' was not found.");

        entity.LabClientId = labClientId;
        entity.LabClient = lab;
        entity.Amount = amount;
        entity.PaidAt = paidAt;
        entity.PaymentMethod = paymentMethod.Trim();
        entity.ReferenceNumber = TrimToNull(referenceNumber);
        entity.Notes = TrimToNull(notes);

        await db.SaveChangesAsync(cancellationToken);
        return MapPayment(entity);
    }

    public async Task DeletePaymentAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.LabAccountPayments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Payment '{id}' was not found.");
        await RequireAccessAsync(entity, "lab_account_payments", "delete", cancellationToken);
        db.LabAccountPayments.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<AccountingPageSetting> GetOrCreateSettingsAsync(CancellationToken cancellationToken)
    {
        var settings = await db.AccountingPageSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings is not null)
            return settings;

        settings = new AccountingPageSetting
        {
            AnnouncementTextAr = string.Empty,
            AnnouncementTextEn = string.Empty,
            TitleAr = string.Empty,
            TitleEn = string.Empty,
            DescriptionAr = string.Empty,
            DescriptionEn = string.Empty,
            IsActive = true
        };
        db.AccountingPageSettings.Add(settings);
        await db.SaveChangesAsync(cancellationToken);
        return settings;
    }

    private async Task RequireAccessAsync<TEntity>(
        TEntity entity,
        string resource,
        string action,
        CancellationToken cancellationToken)
    {
        if (!await accessPolicyEvaluator.CanAccessAsync(entity, resource, action, cancellationToken))
            throw new ApplicationForbiddenException($"You cannot {action} this accounting resource.");
    }

    private async Task<decimal> CalculateBalanceBeforeAsync(
        string labClientId,
        DateTime exclusiveDate,
        CancellationToken cancellationToken)
    {
        var tests = await db.TestRequests.AsNoTracking()
            .Where(r => r.LabClientId == labClientId && r.RequestDate < exclusiveDate)
            .SumAsync(r => r.TotalAmount, cancellationToken);
        var payments = await db.LabAccountPayments.AsNoTracking()
            .Where(p => p.LabClientId == labClientId && p.PaidAt < exclusiveDate)
            .SumAsync(p => p.Amount, cancellationToken);

        return Convert.ToDecimal(tests) - payments;
    }

    private static Period NormalizePeriod(DateTime from, DateTime to)
    {
        var start = from.Date;
        var end = to.Date;
        if (end < start)
            throw new ApplicationBadRequestException("The end date must be greater than or equal to the start date.");
        return new Period(start, end, end.AddDays(1));
    }

    private static string ResolvePatientName(
        string? directPatientId,
        string? externalPatientName,
        IReadOnlyDictionary<string, string> directPatientNames)
    {
        if (!string.IsNullOrWhiteSpace(externalPatientName))
            return externalPatientName;
        if (!string.IsNullOrWhiteSpace(directPatientId) && directPatientNames.TryGetValue(directPatientId, out var name))
            return name;
        return "Unknown patient";
    }

    private static IReadOnlyList<AccountingChartPointDto> BuildChart(
        IReadOnlyList<LabAccountStatementRowDto> rows,
        IReadOnlyList<LabAccountPayment> payments,
        DateTime from,
        DateTime to)
    {
        var daySpan = (to.Date - from.Date).TotalDays;
        if (daySpan <= 45)
        {
            return Enumerable.Range(0, (int)daySpan + 1)
                .Select(offset => from.Date.AddDays(offset))
                .Select(day =>
                {
                    var tests = rows.Where(r => r.RequestDate.Date == day).Sum(r => r.TestPrice);
                    var paid = payments.Where(p => p.PaidAt.Date == day).Sum(p => p.Amount);
                    return new AccountingChartPointDto(day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), tests, paid, tests - paid);
                })
                .ToList();
        }

        var months = new List<DateTime>();
        for (var cursor = new DateTime(from.Year, from.Month, 1); cursor <= to; cursor = cursor.AddMonths(1))
            months.Add(cursor);

        return months.Select(month =>
        {
            var tests = rows.Where(r => r.RequestDate.Year == month.Year && r.RequestDate.Month == month.Month).Sum(r => r.TestPrice);
            var paid = payments.Where(p => p.PaidAt.Year == month.Year && p.PaidAt.Month == month.Month).Sum(p => p.Amount);
            return new AccountingChartPointDto(month.ToString("yyyy-MM", CultureInfo.InvariantCulture), tests, paid, tests - paid);
        }).ToList();
    }

    private static AccountingAnalysisDto BuildAnalysis(IReadOnlyList<LabAccountStatementRowDto> rows)
    {
        var total = rows.Sum(r => r.TestPrice);
        var paid = rows.Sum(r => r.PaymentsApplied);
        var top = rows
            .GroupBy(r => r.TestNameEn)
            .Select(g => new { Name = g.Key, Amount = g.Sum(r => r.TestPrice) })
            .OrderByDescending(x => x.Amount)
            .FirstOrDefault();

        return new AccountingAnalysisDto(
            rows.Count,
            rows.Select(r => r.PatientName).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            rows.Count == 0 ? 0 : total / rows.Count,
            total == 0 ? 0 : Math.Round(paid / total * 100, 2),
            top?.Name ?? string.Empty,
            top?.Amount ?? 0);
    }

    private static IReadOnlyList<string> BuildStatementPdfLines(LabAccountStatementDto statement)
    {
        var lines = new List<string>
        {
            "Lab Account Statement",
            $"Lab: {statement.LabName}",
            $"Period: {statement.PeriodFrom:yyyy-MM-dd} to {statement.PeriodTo:yyyy-MM-dd}",
            $"Outstanding balance until end date: {statement.LabOutstandingBalance:0.00}",
            string.Empty,
            "Totals",
            $"Tests amount: {statement.Totals.TotalTestsAmount:0.00}",
            $"Payments: {statement.Totals.TotalPayments:0.00}",
            $"Remaining: {statement.Totals.RemainingAmount:0.00}",
            string.Empty,
            "Rows"
        };

        foreach (var row in statement.Rows.Take(80))
        {
            lines.Add($"{row.RequestDate:yyyy-MM-dd} | {row.PatientName} | {row.TestNameEn} | {row.TestPrice:0.00} | paid {row.PaymentsApplied:0.00}");
        }

        if (statement.Rows.Count > 80)
            lines.Add($"... {statement.Rows.Count - 80} more rows omitted from this compact PDF.");

        return lines;
    }

    private static void ValidatePayment(decimal amount, string paymentMethod)
    {
        if (amount <= 0)
            throw new ApplicationBadRequestException("Payment amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new ApplicationBadRequestException("Payment method is required.");
    }

    private static string? TrimToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static AccountingPageSettingDto MapSettings(AccountingPageSetting settings) =>
        new(
            settings.Id,
            settings.AnnouncementTextAr,
            settings.AnnouncementTextEn,
            settings.TitleAr,
            settings.TitleEn,
            settings.DescriptionAr,
            settings.DescriptionEn,
            settings.IsActive,
            settings.CreatedAt,
            settings.UpdatedAt);

    private static LabAccountPaymentDto MapPayment(LabAccountPayment payment) =>
        new(
            payment.Id,
            payment.LabClientId,
            payment.LabClient.FullName,
            payment.Amount,
            payment.PaidAt,
            payment.PaymentMethod,
            payment.ReferenceNumber,
            payment.Notes,
            payment.CreatedAt);

    private static LabAccountStatementFileDto MapStatementFile(LabAccountStatementFile file) =>
        new(
            file.Id,
            file.LabClientId,
            file.LabClient.FullName,
            file.PeriodFrom,
            file.PeriodTo,
            file.FileUrl,
            file.OriginalFileName,
            file.Notes,
            file.CreatedAt);

    private sealed record Period(DateTime FromInclusive, DateTime ToInclusive, DateTime ToExclusive);

    private static class SimplePdf
    {
        public static byte[] Create(IReadOnlyList<string> lines)
        {
            var content = new StringBuilder();
            content.AppendLine("BT");
            content.AppendLine("/F1 10 Tf");
            content.AppendLine("50 790 Td");
            foreach (var line in lines)
            {
                content.Append('(').Append(Escape(line)).AppendLine(") Tj");
                content.AppendLine("0 -14 Td");
            }
            content.AppendLine("ET");

            var stream = Encoding.ASCII.GetBytes(content.ToString());
            var objects = new List<string>
            {
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
                $"<< /Length {stream.Length} >>\nstream\n{content}endstream"
            };

            var pdf = new StringBuilder();
            pdf.AppendLine("%PDF-1.4");
            var offsets = new List<int> { 0 };
            foreach (var obj in objects.Select((value, index) => new { value, number = index + 1 }))
            {
                offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
                pdf.AppendLine($"{obj.number} 0 obj");
                pdf.AppendLine(obj.value);
                pdf.AppendLine("endobj");
            }

            var xref = Encoding.ASCII.GetByteCount(pdf.ToString());
            pdf.AppendLine("xref");
            pdf.AppendLine($"0 {objects.Count + 1}");
            pdf.AppendLine("0000000000 65535 f ");
            foreach (var offset in offsets.Skip(1))
                pdf.AppendLine($"{offset:0000000000} 00000 n ");
            pdf.AppendLine("trailer");
            pdf.AppendLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
            pdf.AppendLine("startxref");
            pdf.AppendLine(xref.ToString(CultureInfo.InvariantCulture));
            pdf.AppendLine("%%EOF");
            return Encoding.ASCII.GetBytes(pdf.ToString());
        }

        private static string Escape(string value) =>
            value.Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("(", "\\(", StringComparison.Ordinal)
                .Replace(")", "\\)", StringComparison.Ordinal);
    }
}
