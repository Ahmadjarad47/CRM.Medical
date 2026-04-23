namespace CRM.Medical.API.Contracts.Appointments;

public sealed class LabCreateAppointmentRequest : AppointmentSchedulingRequestBase
{
    public string PatientId { get; init; } = string.Empty;
    public string DoctorId { get; init; } = string.Empty;
}
