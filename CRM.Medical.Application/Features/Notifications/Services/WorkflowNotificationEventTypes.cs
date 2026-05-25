namespace CRM.Medical.Application.Features.Notifications.Services;

public static class WorkflowNotificationEventTypes
{
    public const string TestRequestCreated = "TestRequestCreated";
    public const string TestRequestApproved = "TestRequestApproved";
    public const string TestRequestRejected = "TestRequestRejected";
    public const string TestRequestSampleReceived = "TestRequestSampleReceived";
    public const string TestRequestInProgress = "TestRequestInProgress";
    public const string TestRequestCompleted = "TestRequestCompleted";
    public const string TestResultCreated = "TestResultCreated";
    public const string ReportReady = "ReportReady";
}
