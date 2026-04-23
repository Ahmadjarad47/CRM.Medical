namespace CRM.Medical.API.Contracts.Appointments;

public sealed class PatientBookAppointmentRequest : AppointmentSchedulingRequestBase
{
    public string? DoctorId { get; init; }
    public string? LabPartnerId { get; init; }
}
