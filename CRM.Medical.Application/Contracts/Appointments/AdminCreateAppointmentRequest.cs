namespace CRM.Medical.API.Contracts.Appointments;

public sealed class AdminCreateAppointmentRequest : AppointmentSchedulingRequestBase
{
    public string PatientId { get; init; } = string.Empty;
    public string? DoctorId { get; init; }
    public string? LabPartnerId { get; init; }
}
