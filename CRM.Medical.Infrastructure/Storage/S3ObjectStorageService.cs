using Amazon.S3;
using Amazon.S3.Model;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Configuration.S3;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Storage;

public sealed class S3ObjectStorageService(
    IAmazonS3 s3Client,
    IOptions<S3StorageSettings> options) : IObjectStorageService
{
    private readonly S3StorageSettings _options = options.Value;

    public async Task<string> UploadAsync(
        Stream content,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.BucketName))
            throw new InvalidOperationException(
                "S3 storage is not configured. Set S3:BucketName (or environment S3__BucketName).");

        var safeName = Path.GetFileName(fileName);
        if (string.IsNullOrEmpty(safeName))
            safeName = "attachment";

        var prefix = string.IsNullOrEmpty(_options.KeyPrefix)
            ? string.Empty
            : _options.KeyPrefix.TrimEnd('/') + "/";

        var key = $"{prefix}{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}-{safeName}";

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = content,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            AutoCloseStream = false
        };

        await s3Client.PutObjectAsync(request, cancellationToken);

        return BuildPublicUrl(key);
    }

    private string BuildPublicUrl(string key)
    {
        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
            return $"{_options.PublicBaseUrl!.TrimEnd('/')}/{key}";

        // Virtual-hosted–style URL (AWS). For path-style MinIO, set PublicBaseUrl instead.
        var region = string.IsNullOrWhiteSpace(_options.Region) ? "us-east-1" : _options.Region;
        return $"https://{_options.BucketName}.s3.{region}.amazonaws.com/{key}";
    }
}
