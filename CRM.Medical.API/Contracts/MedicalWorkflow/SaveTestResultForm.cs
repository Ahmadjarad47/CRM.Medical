using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.MedicalWorkflow;

/// <summary>Multipart payload for saving a test result: optional PDF via URL or file upload (not both).</summary>
public sealed class SaveTestResultForm
{
    [Required]
    public DateTime ResultDate { get; set; }

    /// <summary>Optional structured result as JSON text.</summary>
    public string? ResultData { get; set; }

    /// <summary>Public URL when not submitting <see cref="PdfFile"/>.</summary>
    public string? PdfUrl { get; set; }

    public IFormFile? PdfFile { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;
}
