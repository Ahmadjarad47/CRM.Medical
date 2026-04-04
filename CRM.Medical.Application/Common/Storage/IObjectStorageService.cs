namespace CRM.Medical.Application.Common.Storage;

/// <summary>
/// Stores binary objects (e.g. complaint attachments) in S3-compatible storage.
/// </summary>
public interface IObjectStorageService
{
    /// <summary>
    /// Uploads a file and returns the URL to persist on the entity (public base URL or CDN prefix + key).
    /// </summary>
    Task<string> UploadAsync(
        Stream content,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default);
}
