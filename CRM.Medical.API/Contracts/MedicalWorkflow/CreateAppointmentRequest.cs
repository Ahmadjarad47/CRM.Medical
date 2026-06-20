namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class CreateAppointmentRequest
{
    public int AvailabilityId { get; set; }

    public int TestRequestId { get; set; }

    /// <summary>Optional for admin assignment; omitted for self-booking.</summary>
    public string? UserId { get; set; }

    /// <summary>Home, Work, or ComeToUs.</summary>
    public string PatientLocationType { get; set; } = string.Empty;

    /// <summary>Required for Home/Work visits.</summary>
    public double? PatientLatitude { get; set; }

    /// <summary>Required for Home/Work visits.</summary>
    public double? PatientLongitude { get; set; }

    public string? Notes { get; set; }
}
