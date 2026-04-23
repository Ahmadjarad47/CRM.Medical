namespace CRM.Medical.API.Contracts.Admin.Appointments;

public sealed class AppointmentAdminListRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? PatientId { get; init; }
    public string? DoctorId { get; init; }
    public string? LabPartnerId { get; init; }
    public string? Status { get; init; }
}
