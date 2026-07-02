using CRM.Medical.Domain.Constants;

namespace CRM.Medical.Domain.Entities;

public sealed class Appointment : BaseEntity
{
    public int Id { get; set; }

    public int? AvailabilityId { get; set; }

    public int? TestRequestId { get; set; }

    /// <summary>Assigned provider (doctor or lab partner) identity user id.</summary>
    public string ProviderUserId { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Status { get; set; } = AppointmentStatuses.Scheduled;

    public string PatientLocationType { get; set; } = AppointmentPatientLocationTypes.ComeToUs;

    public double? PatientLatitude { get; set; }

    public double? PatientLongitude { get; set; }

    public string? Notes { get; set; }

    public string? AttachmentUrl { get; set; }

    public string? MedicalTestCompletionStatus { get; set; }
}
