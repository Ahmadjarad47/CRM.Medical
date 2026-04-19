using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Domain.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? City { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// When set, the user was created by this account (e.g. doctor/lab/admin). Used for scoped user management.
    /// </summary>
    public string? CreatedByUserId { get; set; }

    public User? CreatedBy { get; set; }

    /// <summary>
    /// Flexible JSONB column for role-specific profile data.
    /// Example: { "doctor": { "specialization": "cardiology" }, "patient": { "bloodType": "O+" } }
    /// </summary>
    public JsonDocument? ProfileMetadata { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public ICollection<Appointment> AppointmentsAsPatient { get; set; } = new List<Appointment>();

    public ICollection<Appointment> AppointmentsAsDoctor { get; set; } = new List<Appointment>();

    public ICollection<Appointment> AppointmentsAsLabPartner { get; set; } = new List<Appointment>();

    public ICollection<Appointment> AppointmentsCreated { get; set; } = new List<Appointment>();

    public ICollection<MedicalTest> MedicalTestsCreated { get; set; } = new List<MedicalTest>();

    public ICollection<TestRequest> TestRequestsCreated { get; set; } = new List<TestRequest>();

    public ICollection<TestResult> TestResultsCreated { get; set; } = new List<TestResult>();
}
