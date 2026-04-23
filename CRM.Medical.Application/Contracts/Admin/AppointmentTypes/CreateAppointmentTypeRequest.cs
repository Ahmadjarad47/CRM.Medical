namespace CRM.Medical.API.Contracts.Admin.AppointmentTypes;

public sealed class CreateAppointmentTypeRequest
{
    public string Name { get; init; } = string.Empty;
}
