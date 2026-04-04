namespace CRM.Medical.API.Controllers.Admin.Models;

public sealed class CreateAppointmentTypeRequest
{
    public string Name { get; init; } = string.Empty;
}

public sealed class UpdateAppointmentTypeRequest
{
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
