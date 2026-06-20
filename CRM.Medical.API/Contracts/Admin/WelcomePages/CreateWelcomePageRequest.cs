using System.ComponentModel.DataAnnotations;
using CRM.Medical.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Admin.WelcomePages;

public sealed class CreateWelcomePageRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name is required.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "Description is required.")]
    public string Description { get; set; } = default!;

    [Required(ErrorMessage = "MediaType is required.")]
    public AdMediaType MediaType { get; set; }

    [Required(ErrorMessage = "Media file is required.")]
    public IFormFile? Media { get; set; }

    public bool IsActive { get; set; } = true;
}
