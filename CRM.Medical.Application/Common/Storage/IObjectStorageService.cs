namespace CRM.Medical.Application.Common.Storage;

/// <summary>
/// Stores binary objects (e.g. complaint attachments) in S3-compatible storage.
/// </summary>
public interface IObjectStorageService
{
    /// <summary>
    /// Uploads a file and returns the URL to persist on the entity (public base URL or CDN prefix + key).
    /// </summary>
    /// <param name="relativeFolder">Logical folder segment after configured key prefix (e.g. <c>slide-cards</c>). No slashes required.</param>
    Task<string> UploadAsync(
        Stream content,
        string contentType,
        string fileName,
        string relativeFolder,
        CancellationToken cancellationToken = default);
}
