using CRM.Medical.Application.Features.Notifications.Services;

namespace CRM.Medical.Infrastructure.Notifications;

internal static class NotificationTemplates
{
    public static (string Title, string Body) Build(WorkflowNotificationRequest request) =>
        request.EventType switch
        {
            WorkflowNotificationEventTypes.TestRequestCreated =>
                ("Test request created", "A new test request related to your account has been created."),
            WorkflowNotificationEventTypes.TestRequestApproved =>
                ("Test request approved", "Your test request has been approved."),
            WorkflowNotificationEventTypes.TestRequestRejected =>
                ("Test request rejected", "Your test request has been rejected."),
            WorkflowNotificationEventTypes.TestRequestSampleReceived =>
                ("Sample received", "The sample for your test request has been received."),
            WorkflowNotificationEventTypes.TestRequestInProgress =>
                ("Test in progress", "Your test request is now in progress."),
            WorkflowNotificationEventTypes.TestRequestCompleted =>
                ("Test completed", "Your test request has been completed."),
            WorkflowNotificationEventTypes.TestResultCreated =>
                ("Test result created", "A result has been created for your test request."),
            WorkflowNotificationEventTypes.ReportReady =>
                ("Report ready", "Your test report is ready."),
            _ =>
                ("Notification", "You have a new notification.")
        };
}
