namespace CRM.Medical.API.Contracts.Appointments;

public sealed class DoctorCreateAppointmentRequest : AppointmentSchedulingRequestBase
{
    public string PatientId { get; init; } = string.Empty;
    public string LabPartnerId { get; init; } = string.Empty;
}
