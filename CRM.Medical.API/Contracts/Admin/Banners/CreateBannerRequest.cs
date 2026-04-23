using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Admin.Banners;

public sealed class CreateBannerRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title is required.")]
    public string Title { get; set; } = default!;

    [Required(ErrorMessage = "Type is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Type is required.")]
    public string Type { get; set; } = default!;

    public string? InternalLink { get; set; }

    public string? ExternalLink { get; set; }

    [Required(ErrorMessage = "TargetType is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "TargetType is required.")]
    public string TargetType { get; set; } = default!;

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Location is required.")]
    public string Location { get; set; } = default!;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string? VisibilityRulesJson { get; set; }

    [Required(ErrorMessage = "StartDate is required.")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "EndDate is required.")]
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Media file is required.")]
    public IFormFile? Media { get; set; }
}
