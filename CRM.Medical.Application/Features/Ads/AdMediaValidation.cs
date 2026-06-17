using CRM.Medical.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Ads;

internal static class AdMediaValidation
{
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".webm"
    };

    public static bool MatchesMediaType(IFormFile? file, AdMediaType mediaType)
    {
        if (file is not { Length: > 0 })
            return false;

        var extension = Path.GetExtension(file.FileName);
        return mediaType switch
        {
            AdMediaType.Image => ImageExtensions.Contains(extension),
            AdMediaType.Video => VideoExtensions.Contains(extension),
            _ => false
        };
    }
}
