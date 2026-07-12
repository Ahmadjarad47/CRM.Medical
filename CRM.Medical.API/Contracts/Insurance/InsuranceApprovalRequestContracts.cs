using System.ComponentModel.DataAnnotations;
using CRM.Medical.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Insurance;

public sealed class CreateInsuranceApprovalRequest
{
    [Required(ErrorMessage = "Insured name is required.")]
    public string InsuredName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Insurance number is required.")]
    public string InsuranceNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required.")]
    public string MobileNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Insurance card image is required.")]
    public IFormFile? InsuranceCardImage { get; set; }

    [Required(ErrorMessage = "Prescription image is required.")]
    public IFormFile? PrescriptionImage { get; set; }
}

public sealed record UpdateInsuranceApprovalRequestStatusRequest(
    InsuranceApprovalRequestStatus Status,
    string? Notes,
    string? RejectionReason);
