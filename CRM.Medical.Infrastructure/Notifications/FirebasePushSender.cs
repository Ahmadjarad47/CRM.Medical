using CRM.Medical.Application.Features.Notifications.Services;
using CRM.Medical.Infrastructure.Configuration;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Notifications;

internal sealed class FirebasePushSender(
    IOptions<FirebaseOptions> options,
    ILogger<FirebasePushSender> logger) : IFirebasePushSender
{
    private const int FirebaseMulticastBatchSize = 500;
    private readonly Lazy<FirebaseApp?> _firebaseApp = new(() => CreateApp(options.Value, logger));

    public async Task<IReadOnlyList<FirebasePushSendResult>> SendAsync(
        IReadOnlyList<string> tokens,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        if (tokens.Count == 0)
            return [];

        var app = _firebaseApp.Value;
        if (app is null)
            return tokens.Select(token => new FirebasePushSendResult(token, false, false, "FirebaseNotConfigured")).ToList();

        var messaging = FirebaseMessaging.GetMessaging(app);
        var results = new List<FirebasePushSendResult>(tokens.Count);

        foreach (var batch in tokens.Chunk(FirebaseMulticastBatchSize))
        {
            var message = new MulticastMessage
            {
                Tokens = batch.ToList(),
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data is null
                    ? new Dictionary<string, string>(StringComparer.Ordinal)
                    : data.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal)
            };

            var response = await messaging.SendEachForMulticastAsync(message, cancellationToken);
            for (var i = 0; i < message.Tokens.Count; i++)
            {
                var sendResponse = response.Responses[i];
                var errorCode = sendResponse.Exception?.MessagingErrorCode.ToString();
                var shouldDeactivateToken =
                    !sendResponse.IsSuccess &&
                    sendResponse.Exception?.MessagingErrorCode is MessagingErrorCode.Unregistered or MessagingErrorCode.InvalidArgument;

                results.Add(new FirebasePushSendResult(
                    message.Tokens[i],
                    sendResponse.IsSuccess,
                    shouldDeactivateToken,
                    errorCode));
            }
        }

        return results;
    }

    private static FirebaseApp? CreateApp(FirebaseOptions options, ILogger logger)
    {
        var credential = BuildCredential(options, logger);
        if (credential is null)
            return null;

        var appOptions = new AppOptions
        {
            Credential = credential
        };

        if (!string.IsNullOrWhiteSpace(options.ProjectId))
            appOptions.ProjectId = options.ProjectId.Trim();

        return FirebaseApp.Create(appOptions, $"crm-medical-push-{Guid.NewGuid():N}");
    }

    private static GoogleCredential? BuildCredential(FirebaseOptions options, ILogger logger)
    {
        if (!string.IsNullOrWhiteSpace(options.CredentialsJson))
            return GoogleCredential.FromJson(options.CredentialsJson);

        if (!string.IsNullOrWhiteSpace(options.CredentialsPath))
        {
            var fullPath = Environment.ExpandEnvironmentVariables(options.CredentialsPath.Trim());
            if (File.Exists(fullPath))
                return GoogleCredential.FromFile(fullPath);

            logger.LogWarning("Firebase credentials path was configured but file was not found: {Path}", fullPath);
            return null;
        }

        var envPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_PATH");
        if (!string.IsNullOrWhiteSpace(envPath))
        {
            var fullPath = Environment.ExpandEnvironmentVariables(envPath.Trim());
            if (File.Exists(fullPath))
                return GoogleCredential.FromFile(fullPath);

            logger.LogWarning("FIREBASE_CREDENTIALS_PATH was set but file was not found: {Path}", fullPath);
            return null;
        }

        var envJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
        if (!string.IsNullOrWhiteSpace(envJson))
            return GoogleCredential.FromJson(envJson);

        logger.LogWarning("Firebase push sender is not configured. Notifications will be skipped.");
        return null;
    }
}
