using CRM.Medical.Application.Features.Appointments;

namespace CRM.Medical.API.Contracts.Appointments;

public abstract class AppointmentSchedulingRequestBase
{
    public int AppointmentTypeId { get; init; }

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateTime Slot { get; init; }
    public string LocationType { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }

    public AppointmentFormFields ToFormFields() =>
        new(AppointmentTypeId, Name, Description, Notes, Slot, LocationType, Address, Latitude, Longitude);
}
