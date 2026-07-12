using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Admin.SlideCards;

public sealed class CreateSlideCardRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title is required.")]
    public string Title { get; init; } = default!;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "Description is required.")]
    public string Description { get; init; } = default!;

    public decimal Price { get; init; }

    public decimal Discount { get; init; }

    [Required(ErrorMessage = "ExpiryDate is required.")]
    public DateTime? ExpiryDate { get; init; }

    [Required(ErrorMessage = "Badge is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Badge is required.")]
    public string Badge { get; init; } = default!;

    [Required(ErrorMessage = "DetailPageLink is required.")]
    [StringLength(2048, MinimumLength = 1, ErrorMessage = "DetailPageLink is required.")]
    public string DetailPageLink { get; init; } = default!;

    public int DisplayOrder { get; init; }

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "Image is required.")]
    public IFormFile? Image { get; init; }
}

public sealed class UpdateSlideCardRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title is required.")]
    public string Title { get; init; } = default!;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "Description is required.")]
    public string Description { get; init; } = default!;

    public decimal Price { get; init; }

    public decimal Discount { get; init; }

    [Required(ErrorMessage = "ExpiryDate is required.")]
    public DateTime? ExpiryDate { get; init; }

    [Required(ErrorMessage = "Badge is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Badge is required.")]
    public string Badge { get; init; } = default!;

    [Required(ErrorMessage = "DetailPageLink is required.")]
    [StringLength(2048, MinimumLength = 1, ErrorMessage = "DetailPageLink is required.")]
    public string DetailPageLink { get; init; } = default!;

    public int DisplayOrder { get; init; }

    public bool IsActive { get; init; }

    public IFormFile? Image { get; init; }
}
