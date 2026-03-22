using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Domain.Entities;

/// <summary>
/// Application user stored in the <c>Users</c> table; extends ASP.NET Core Identity.
/// </summary>
public sealed class User : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public UserType UserType { get; set; } = UserType.Patient;

    // Shared profile details
    public DateOnly? DateOfBirth { get; set; }
    public string? PhoneSecondary { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Patient fields
    public string? NationalIdNumber { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }

    // Doctor fields
    public string? MedicalLicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public string? ClinicName { get; set; }

    // Lab fields
    public string? LabName { get; set; }
    public string? LabLicenseNumber { get; set; }
    public string? LabContactName { get; set; }
    public string? LabContactPhone { get; set; }

    // Special account fields
    public string? SpecialAccountCode { get; set; }
    public string? SpecialNotes { get; set; }
}
