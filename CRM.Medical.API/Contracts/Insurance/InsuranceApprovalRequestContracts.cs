using CRM.Medical.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Insurance;

public sealed class CreateInsuranceApprovalRequest
{
    public string InsuredName { get; set; } = string.Empty;

    public string InsuranceNumber { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public IFormFile InsuranceCardImage { get; set; } = null!;

    public IFormFile PrescriptionImage { get; set; } = null!;
}

public sealed record UpdateInsuranceApprovalRequestStatusRequest(
    InsuranceApprovalRequestStatus Status,
    string? Notes,
    string? RejectionReason);
