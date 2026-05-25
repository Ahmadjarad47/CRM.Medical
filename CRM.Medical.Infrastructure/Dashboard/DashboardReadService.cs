using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Dashboard.Queries.GetDashboard;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Dashboard;

public sealed class DashboardReadService(MedicalDbContext dbContext, IDateTimeProvider dateTimeProvider)
    : IDashboardReadService
{
    private readonly MedicalDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<DashboardResponse> GetDashboardAsync(string role, string userId, CancellationToken cancellationToken)
    {
        var scopedTestRequests = BuildScopedTestRequestQuery(role, userId);
        var scopedTestResults = BuildScopedTestResultQuery(role, userId);
        var scopedExternalPatients = BuildScopedExternalPatientQuery(role, userId);
        var scopedComplaints = BuildScopedComplaintQuery(role, userId);
        var scopedTemplates = BuildScopedTemplateQuery(role, userId);
        var scopedMedicalTests = BuildScopedMedicalTestQuery(scopedTestRequests, role);

        var totalTestRequests = await scopedTestRequests.CountAsync(cancellationToken);
        var totalResults = await scopedTestResults.CountAsync(cancellationToken);
        var completedResults = await scopedTestResults.CountAsync(x => x.Status == "Completed", cancellationToken);
        var totalRevenue = totalTestRequests == 0
            ? 0d
            : await scopedTestRequests.SumAsync(x => (double?)x.TotalAmount, cancellationToken) ?? 0d;

        var linkedPatientCount = await scopedTestRequests
            .Where(x => x.DirectPatientId != null)
            .Select(x => x.DirectPatientId!)
            .Distinct()
            .CountAsync(cancellationToken);

        var doctorCount = await scopedTestRequests
            .Where(x => x.DoctorId != null)
            .Select(x => x.DoctorId!)
            .Distinct()
            .CountAsync(cancellationToken);

        var labPartnerCount = await scopedTestRequests
            .Where(x => x.LabClientId != null)
            .Select(x => x.LabClientId!)
            .Distinct()
            .CountAsync(cancellationToken);

        var summary = new DashboardSummary(
            TotalUsers: role == UserRoles.Admin ? await _dbContext.Users.AsNoTracking().CountAsync(cancellationToken) : linkedPatientCount + doctorCount + labPartnerCount,
            TotalDoctors: role == UserRoles.Admin ? await CountUsersInRole(UserRoles.Doctor, cancellationToken) : doctorCount,
            TotalPatients: role == UserRoles.Admin ? await CountUsersInRole(UserRoles.Patient, cancellationToken) : linkedPatientCount,
            TotalLabPartners: role == UserRoles.Admin ? await CountUsersInRole(UserRoles.LabPartner, cancellationToken) : labPartnerCount,
            TotalMedicalTests: await scopedMedicalTests.CountAsync(cancellationToken),
            TotalTestRequests: totalTestRequests,
            TotalResults: totalResults,
            CompletedResults: completedResults,
            TotalExternalPatients: await scopedExternalPatients.CountAsync(cancellationToken),
            TotalComplaints: await scopedComplaints.CountAsync(cancellationToken),
            TotalTemplates: await scopedTemplates.CountAsync(cancellationToken),
            TotalRevenue: totalRevenue);

        var charts = new DashboardCharts(
            RequestStatus: await BuildCountChart(
                scopedTestRequests.Select(x => x.Status),
                "Unknown",
                cancellationToken),
            ResultStatus: await BuildCountChart(
                scopedTestResults.Select(x => x.Status),
                "Unknown",
                cancellationToken),
            TestCategoryBreakdown: await BuildCountChart(
                scopedMedicalTests.Select(x => x.Category),
                "Uncategorized",
                cancellationToken),
            MonthlyRequests: await BuildMonthlyRequestChart(scopedTestRequests, cancellationToken),
            MonthlyRevenue: await BuildMonthlyRevenueChart(scopedTestRequests, cancellationToken),
            UserRoleDistribution: await BuildRoleDistribution(role, scopedTestRequests, cancellationToken));

        var recent = new DashboardRecentData(
            TestRequests: await scopedTestRequests
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .Select(x => new RecentTestRequestItem(
                    x.Id,
                    x.RequestDate,
                    x.Status,
                    x.TotalAmount,
                    x.MedicalTest.NameEn,
                    x.DoctorId,
                    x.LabClientId,
                    x.DirectPatientId,
                    x.ExternalPatient != null ? x.ExternalPatient.FullName : null))
                .Take(10)
                .ToListAsync(cancellationToken),
            TestResults: await scopedTestResults
                .OrderByDescending(x => x.ResultDate)
                .ThenByDescending(x => x.Id)
                .Select(x => new RecentTestResultItem(
                    x.Id,
                    x.TestRequestId,
                    x.ResultDate,
                    x.Status,
                    x.PdfUrl,
                    x.TestRequest.MedicalTest.NameEn))
                .Take(10)
                .ToListAsync(cancellationToken),
            Complaints: await scopedComplaints
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Select(x => new RecentComplaintItem(
                    x.Id,
                    x.Title,
                    x.Status,
                    x.UserId,
                    x.CreatedAt))
                .Take(10)
                .ToListAsync(cancellationToken));

        return new DashboardResponse(
            new DashboardScope(
                Role: role,
                UserId: userId,
                IsGlobalDashboard: role == UserRoles.Admin,
                AppliedFilters: BuildAppliedFilters(role, userId)),
            summary,
            charts,
            recent);
    }

    private IQueryable<TestRequest> BuildScopedTestRequestQuery(string role, string userId)
    {
        var query = _dbContext.TestRequests
            .AsNoTracking()
            .Include(x => x.MedicalTest)
            .Include(x => x.ExternalPatient)
            .AsQueryable();

        return role switch
        {
            UserRoles.Admin => query,
            UserRoles.Doctor => query.Where(x => x.DoctorId == userId),
            UserRoles.LabPartner => query.Where(x => x.LabClientId == userId),
            UserRoles.Patient => query.Where(x =>
                x.DirectPatientId == userId ||
                (x.ExternalPatient != null && x.ExternalPatient.LinkedDirectPatientId == userId)),
            _ => query.Where(_ => false)
        };
    }

    private IQueryable<TestResult> BuildScopedTestResultQuery(string role, string userId)
    {
        var query = _dbContext.TestResults
            .AsNoTracking()
            .Include(x => x.TestRequest)
                .ThenInclude(x => x.MedicalTest)
            .Include(x => x.TestRequest)
                .ThenInclude(x => x.ExternalPatient)
            .AsQueryable();

        return role switch
        {
            UserRoles.Admin => query,
            UserRoles.Doctor => query.Where(x => x.TestRequest.DoctorId == userId),
            UserRoles.LabPartner => query.Where(x => x.TestRequest.LabClientId == userId),
            UserRoles.Patient => query.Where(x =>
                x.TestRequest.DirectPatientId == userId ||
                (x.TestRequest.ExternalPatient != null && x.TestRequest.ExternalPatient.LinkedDirectPatientId == userId)),
            _ => query.Where(_ => false)
        };
    }

    private IQueryable<ExternalPatient> BuildScopedExternalPatientQuery(string role, string userId)
    {
        var query = _dbContext.ExternalPatients.AsNoTracking().AsQueryable();

        return role switch
        {
            UserRoles.Admin => query,
            UserRoles.Doctor => query.Where(x => x.TestRequests.Any(r => r.DoctorId == userId)),
            UserRoles.LabPartner => query.Where(x => x.TestRequests.Any(r => r.LabClientId == userId)),
            UserRoles.Patient => query.Where(x => x.LinkedDirectPatientId == userId),
            _ => query.Where(_ => false)
        };
    }

    private IQueryable<Complaint> BuildScopedComplaintQuery(string role, string userId)
    {
        var query = _dbContext.Complaints.AsNoTracking().AsQueryable();
        return role == UserRoles.Admin ? query : query.Where(x => x.UserId == userId);
    }

    private IQueryable<Template> BuildScopedTemplateQuery(string role, string userId)
    {
        var query = _dbContext.Templates.AsNoTracking().AsQueryable();
        return role == UserRoles.Admin
            ? query
            : query.Where(x => x.Role == role || x.CreatedByUserId == userId);
    }

    private IQueryable<MedicalTest> BuildScopedMedicalTestQuery(IQueryable<TestRequest> scopedTestRequests, string role)
    {
        var query = _dbContext.MedicalTests.AsNoTracking().AsQueryable();
        return role == UserRoles.Admin
            ? query
            : query.Where(x => scopedTestRequests.Select(r => r.MedicalTestId).Contains(x.Id));
    }

    private async Task<int> CountUsersInRole(string roleName, CancellationToken cancellationToken)
    {
        return await
            (from userRole in _dbContext.Set<IdentityUserRole<string>>().AsNoTracking()
             join role in _dbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
             where role.Name == roleName
             select userRole.UserId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    private async Task<List<DashboardTimeSeriesItem>> BuildMonthlyRequestChart(
        IQueryable<TestRequest> scopedTestRequests,
        CancellationToken cancellationToken)
    {
        var startMonth = GetUtcMonthStart(_dateTimeProvider.UtcNow).AddMonths(-5);

        var rows = await scopedTestRequests
            .Where(x => x.RequestDate >= startMonth)
            .GroupBy(x => new { x.RequestDate.Year, x.RequestDate.Month })
            .Select(x => new { x.Key.Year, x.Key.Month, Count = x.Count() })
            .ToListAsync(cancellationToken);

        return BuildMonthSeries(startMonth, rows.Select(x => (x.Year, x.Month, (double)x.Count)));
    }

    private async Task<List<DashboardTimeSeriesItem>> BuildMonthlyRevenueChart(
        IQueryable<TestRequest> scopedTestRequests,
        CancellationToken cancellationToken)
    {
        var startMonth = GetUtcMonthStart(_dateTimeProvider.UtcNow).AddMonths(-5);

        var rows = await scopedTestRequests
            .Where(x => x.RequestDate >= startMonth)
            .GroupBy(x => new { x.RequestDate.Year, x.RequestDate.Month })
            .Select(x => new { x.Key.Year, x.Key.Month, Revenue = x.Sum(y => y.TotalAmount) })
            .ToListAsync(cancellationToken);

        return BuildMonthSeries(startMonth, rows.Select(x => (x.Year, x.Month, x.Revenue)));
    }

    private static async Task<List<DashboardChartItem>> BuildCountChart(
        IQueryable<string?> source,
        string fallbackLabel,
        CancellationToken cancellationToken)
    {
        var rows = await source
            .Select(value => value == null || value == string.Empty ? fallbackLabel : value)
            .GroupBy(value => value)
            .Select(group => new { Label = group.Key, Count = group.Count() })
            .OrderByDescending(group => group.Count)
            .ToListAsync(cancellationToken);

        return rows
            .Select(row => new DashboardChartItem(row.Label!, row.Count, 0d))
            .ToList();
    }

    private async Task<List<DashboardChartItem>> BuildRoleDistribution(
        string role,
        IQueryable<TestRequest> scopedTestRequests,
        CancellationToken cancellationToken)
    {
        if (role == UserRoles.Admin)
        {
            return
            [
                new DashboardChartItem(UserRoles.Admin, await CountUsersInRole(UserRoles.Admin, cancellationToken), 0d),
                new DashboardChartItem(UserRoles.Doctor, await CountUsersInRole(UserRoles.Doctor, cancellationToken), 0d),
                new DashboardChartItem(UserRoles.Patient, await CountUsersInRole(UserRoles.Patient, cancellationToken), 0d),
                new DashboardChartItem(UserRoles.LabPartner, await CountUsersInRole(UserRoles.LabPartner, cancellationToken), 0d)
            ];
        }

        var doctors = await scopedTestRequests
            .Where(x => x.DoctorId != null)
            .Select(x => x.DoctorId!)
            .Distinct()
            .CountAsync(cancellationToken);

        var patients = await scopedTestRequests
            .Where(x => x.DirectPatientId != null)
            .Select(x => x.DirectPatientId!)
            .Distinct()
            .CountAsync(cancellationToken);

        var labPartners = await scopedTestRequests
            .Where(x => x.LabClientId != null)
            .Select(x => x.LabClientId!)
            .Distinct()
            .CountAsync(cancellationToken);

        return
        [
            new DashboardChartItem(UserRoles.Doctor, doctors, 0d),
            new DashboardChartItem(UserRoles.Patient, patients, 0d),
            new DashboardChartItem(UserRoles.LabPartner, labPartners, 0d)
        ];
    }

    private static List<DashboardTimeSeriesItem> BuildMonthSeries(
        DateTime startMonth,
        IEnumerable<(int Year, int Month, double Value)> rows)
    {
        var values = rows.ToDictionary(x => $"{x.Year:D4}-{x.Month:D2}", x => x.Value);
        var series = new List<DashboardTimeSeriesItem>(6);

        for (var i = 0; i < 6; i++)
        {
            var month = startMonth.AddMonths(i);
            var key = $"{month.Year:D4}-{month.Month:D2}";
            series.Add(new DashboardTimeSeriesItem(
                key,
                month.ToString("MMM yyyy"),
                values.GetValueOrDefault(key)));
        }

        return series;
    }

    private static DateTime GetUtcMonthStart(DateTime utcDateTime) =>
        new(utcDateTime.Year, utcDateTime.Month, 1, 0, 0, 0, DateTimeKind.Utc);

    private static IReadOnlyDictionary<string, string> BuildAppliedFilters(string role, string userId)
    {
        return role switch
        {
            UserRoles.Admin => new Dictionary<string, string> { ["scope"] = "all" },
            UserRoles.Doctor => new Dictionary<string, string>
            {
                ["scope"] = "doctor",
                ["doctorId"] = userId
            },
            UserRoles.LabPartner => new Dictionary<string, string>
            {
                ["scope"] = "labPartner",
                ["labPartnerId"] = userId
            },
            UserRoles.Patient => new Dictionary<string, string>
            {
                ["scope"] = "patient",
                ["patientId"] = userId
            },
            _ => new Dictionary<string, string> { ["scope"] = "unknown" }
        };
    }
}
