using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.Insurance.DTOs;

public sealed record InsuranceApprovalRequestDto(
    int Id,
    string PatientId,
    string PatientName,
    string InsuredName,
    string InsuranceNumber,
    string MobileNumber,
    InsuranceApprovalRequestStatus Status,
    string? Notes,
    string? RejectionReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record InsuranceApprovalRequestDetailsDto(
    int Id,
    string PatientId,
    string PatientName,
    string InsuredName,
    string InsuranceNumber,
    string MobileNumber,
    string InsuranceCardImageUrl,
    string InsuranceCardOriginalFileName,
    string PrescriptionImageUrl,
    string PrescriptionOriginalFileName,
    InsuranceApprovalRequestStatus Status,
    string? Notes,
    string? RejectionReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record InsuranceApprovalSubmissionResponseDto(string Message);
