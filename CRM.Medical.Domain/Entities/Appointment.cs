using CRM.Medical.Domain.Constants;

namespace CRM.Medical.Domain.Entities;

public sealed class Appointment
{
    public int Id { get; set; }

    /// <summary>اسم الحجز</summary>
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>ملاحظات</summary>
    public string? Notes { get; set; }

    public DateTime Slot { get; set; }

    public string LocationType { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string Status { get; set; } = string.Empty;

    public int AppointmentTypeId { get; set; }
    public AppointmentType AppointmentType { get; set; } = null!;

    public string PatientId { get; set; } = string.Empty;
    public User Patient { get; set; } = null!;

    public string? DoctorId { get; set; }
    public User? Doctor { get; set; }

    public string? LabPartnerId { get; set; }
    public User? LabPartner { get; set; }

    /// <summary>Optional 1:1 link to a medical test instance for this appointment.</summary>
    public int? MedicalTestId { get; set; }
    public MedicalTest? MedicalTest { get; set; }

    /// <summary>
    /// When <see cref="MedicalTestId"/> is set, tracks whether the linked medical test work is finished (see <see cref="AppointmentMedicalTestCompletionStatuses"/>).
    /// Must be null when <see cref="MedicalTestId"/> is null.
    /// </summary>
    public string? MedicalTestCompletionStatus { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;
    public User CreatedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
