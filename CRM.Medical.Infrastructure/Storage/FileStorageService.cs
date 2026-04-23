using System.Globalization;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Configuration.S3;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Storage;

public sealed class FileStorageService(
    IObjectStorageService objectStorage,
    IOptions<S3StorageSettings> options) : IFileStorageService
{
    private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".bat", ".cmd", ".com", ".msi", ".dll", ".pif", ".scr", ".vbs", ".sh", ".hta", ".ps1", ".jar"
    };

    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    private static readonly HashSet<string> ImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp"
    };

    private static readonly HashSet<string> PdfContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf"
    };

    private static readonly HashSet<string> GeneralAttachmentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx"
    };

    private static readonly HashSet<string> GeneralAttachmentContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    private readonly S3StorageSettings _settings = options.Value;

    public Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default) =>
        UploadInternalAsync(file, "images", ImageExtensions, ImageContentTypes, cancellationToken);

    public Task<string> UploadPdfAsync(IFormFile file, CancellationToken cancellationToken = default) =>
        UploadInternalAsync(file, "pdfs", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".pdf" },
            PdfContentTypes, cancellationToken);

    public Task<string> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentException("Folder is required.", nameof(folder));

        var f = folder.Trim().Trim('/');
        var extSet = new HashSet<string>(GeneralAttachmentExtensions, StringComparer.OrdinalIgnoreCase);
        var mimeSet = new HashSet<string>(GeneralAttachmentContentTypes, StringComparer.OrdinalIgnoreCase);
        if (f.Equals("banners", StringComparison.OrdinalIgnoreCase))
        {
            extSet.UnionWith([".mp4", ".webm"]);
            mimeSet.UnionWith(["video/mp4", "video/webm"]);
        }

        return UploadInternalAsync(file, f, extSet, mimeSet, cancellationToken);
    }

    private async Task<string> UploadInternalAsync(
        IFormFile file,
        string relativeFolder,
        HashSet<string> allowedExtensions,
        HashSet<string> allowedContentTypes,
        CancellationToken cancellationToken)
    {
        ValidateCore(file, allowedExtensions, allowedContentTypes);

        var contentType = NormalizeContentType(file);
        await using var stream = file.OpenReadStream();
        return await objectStorage.UploadAsync(stream, contentType, file.FileName, relativeFolder, cancellationToken);
    }

    private void ValidateCore(
        IFormFile file,
        HashSet<string> allowedExtensions,
        HashSet<string> allowedContentTypes)
    {
        if (file.Length <= 0)
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(IFormFile), "File is empty.")]);

        if (file.Length > _settings.MaxAttachmentBytes)
        {
            throw new ValidationException([
                new FluentValidation.Results.ValidationFailure(
                    nameof(IFormFile),
                    string.Format(CultureInfo.InvariantCulture,
                        "File must not exceed {0} bytes.", _settings.MaxAttachmentBytes))
            ]);
        }

        var ext = Path.GetExtension(file.FileName);
        if (!string.IsNullOrEmpty(ext) && BlockedExtensions.Contains(ext))
        {
            throw new ValidationException([
                new FluentValidation.Results.ValidationFailure(nameof(IFormFile), "This file type is not allowed.")
            ]);
        }

        if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
        {
            throw new ValidationException([
                new FluentValidation.Results.ValidationFailure(nameof(IFormFile), "File extension is not allowed.")
            ]);
        }

        var ct = file.ContentType;
        if (string.IsNullOrWhiteSpace(ct))
            return;

        if (ct.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
            return;

        if (!allowedContentTypes.Contains(ct))
        {
            throw new ValidationException([
                new FluentValidation.Results.ValidationFailure(nameof(IFormFile), "Content type is not allowed for this upload.")
            ]);
        }
    }

    private static string NormalizeContentType(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName);
        if (!string.IsNullOrWhiteSpace(file.ContentType)
            && !file.ContentType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
            return file.ContentType;

        if (ImageExtensions.Contains(ext))
        {
            if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase)) return "image/png";
            if (ext.Equals(".gif", StringComparison.OrdinalIgnoreCase)) return "image/gif";
            if (ext.Equals(".webp", StringComparison.OrdinalIgnoreCase)) return "image/webp";
            return "image/jpeg";
        }

        if (ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            return "application/pdf";

        if (ext.Equals(".doc", StringComparison.OrdinalIgnoreCase))
            return "application/msword";

        if (ext.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        if (ext.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
            return "video/mp4";

        if (ext.Equals(".webm", StringComparison.OrdinalIgnoreCase))
            return "video/webm";

        return string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
    }
}
