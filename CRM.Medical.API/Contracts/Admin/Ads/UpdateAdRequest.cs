using System.ComponentModel.DataAnnotations;
using CRM.Medical.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Admin.Ads;

public sealed class UpdateAdRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name is required.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "Description is required.")]
    public string Description { get; set; } = default!;

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public double? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public double? Longitude { get; set; }

    [Required(ErrorMessage = "Address name is required.")]
    [StringLength(300, MinimumLength = 1, ErrorMessage = "Address name is required.")]
    public string AddressName { get; set; } = default!;

    [Required(ErrorMessage = "MediaType is required.")]
    public AdMediaType MediaType { get; set; }

    [Required(ErrorMessage = "DisplayMode is required.")]
    public DisplayMode DisplayMode { get; set; }

    public IFormFile? Media { get; set; }
}
