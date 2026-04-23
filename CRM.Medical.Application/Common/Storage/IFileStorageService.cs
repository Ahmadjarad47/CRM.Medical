using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Common.Storage;

/// <summary>
/// Validates and uploads multipart files to object storage; returns the public URL (or key-derived URL) to persist.
/// </summary>
public interface IFileStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

    Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default);

    Task<string> UploadPdfAsync(IFormFile file, CancellationToken cancellationToken = default);
}
