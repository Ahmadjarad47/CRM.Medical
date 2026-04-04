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

    public string CreatedByUserId { get; set; } = string.Empty;
    public User CreatedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
