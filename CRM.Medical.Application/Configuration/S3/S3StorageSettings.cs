namespace CRM.Medical.Application.Configuration.S3;

public sealed class S3StorageSettings
{
    public const string SectionName = "S3";

    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Optional endpoint for MinIO and other S3-compatible servers.</summary>
    public string? ServiceUrl { get; set; }

    public bool ForcePathStyle { get; set; }

    /// <summary>Key prefix inside the bucket, e.g. <c>complaints/</c>.</summary>
    public string KeyPrefix { get; set; } = "complaints/";

    /// <summary>
    /// Base URL for stored files (public bucket, CDN, or reverse proxy). If empty, a default S3 URL is built from bucket and region.
    /// </summary>
    public string? PublicBaseUrl { get; set; }

    public long MaxAttachmentBytes { get; set; } = 10 * 1024 * 1024;
}
