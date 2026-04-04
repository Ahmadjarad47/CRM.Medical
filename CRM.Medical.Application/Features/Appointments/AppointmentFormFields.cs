namespace CRM.Medical.Application.Features.Appointments;

/// <summary>Shared scheduling payload for create flows (name, description, notes, time, location).</summary>
public sealed record AppointmentFormFields(
    int AppointmentTypeId,
    string Name,
    string Description,
    string? Notes,
    DateTime Slot,
    string LocationType,
    string Address,
    double? Latitude,
    double? Longitude);
