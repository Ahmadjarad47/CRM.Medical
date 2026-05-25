namespace CRM.Medical.Application.Features.Dashboard.Queries.GetDashboard;

public sealed record DashboardResponse(
    DashboardScope Scope,
    DashboardSummary Summary,
    DashboardWorkflow Workflow,
    DashboardCharts Charts,
    DashboardRecentData Recent);

public sealed record DashboardScope(
    string Role,
    string UserId,
    bool IsGlobalDashboard,
    IReadOnlyDictionary<string, string> AppliedFilters);

public sealed record DashboardSummary(
    int TotalUsers,
    int TotalDoctors,
    int TotalPatients,
    int TotalLabPartners,
    int TotalMedicalTests,
    int TotalTestRequests,
    int TotalResults,
    int CompletedResults,
    int TotalExternalPatients,
    int TotalComplaints,
    int TotalTemplates,
    double TotalRevenue);

public sealed record DashboardCharts(
    IReadOnlyList<DashboardChartItem> RequestStatus,
    IReadOnlyList<DashboardChartItem> ResultStatus,
    IReadOnlyList<DashboardChartItem> TestCategoryBreakdown,
    IReadOnlyList<DashboardTimeSeriesItem> MonthlyRequests,
    IReadOnlyList<DashboardTimeSeriesItem> MonthlyRevenue,
    IReadOnlyList<DashboardChartItem> UserRoleDistribution);

public sealed record DashboardWorkflow(
    string Title,
    string LiveStatusLabel,
    IReadOnlyList<DashboardWorkflowStageItem> Stages);

public sealed record DashboardWorkflowStageItem(
    string Key,
    string Title,
    string Subtitle,
    int Count,
    string Badge,
    string Icon,
    string Accent,
    int SortOrder);

public sealed record DashboardChartItem(string Label, int Count, double Value);

public sealed record DashboardTimeSeriesItem(string Key, string Label, double Value);

public sealed record DashboardRecentData(
    IReadOnlyList<RecentTestRequestItem> TestRequests,
    IReadOnlyList<RecentTestResultItem> TestResults,
    IReadOnlyList<RecentComplaintItem> Complaints);

public sealed record RecentTestRequestItem(
    int Id,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string MedicalTestName,
    string? DoctorId,
    string? LabPartnerId,
    string? DirectPatientId,
    string? ExternalPatientName);

public sealed record RecentTestResultItem(
    int Id,
    int TestRequestId,
    DateTime ResultDate,
    string Status,
    string? PdfUrl,
    string MedicalTestName);

public sealed record RecentComplaintItem(
    int Id,
    string Title,
    string Status,
    string UserId,
    DateTime CreatedAt);
