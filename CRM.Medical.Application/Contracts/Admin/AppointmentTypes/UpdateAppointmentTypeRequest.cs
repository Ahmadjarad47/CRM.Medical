namespace CRM.Medical.API.Contracts.Admin.AppointmentTypes;

public sealed class UpdateAppointmentTypeRequest
{
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
