namespace CRM.Medical.Application.Features.Appointments.DTOs;

public sealed record AppointmentDto(
    int Id,
    int AppointmentTypeId,
    string AppointmentTypeName,
    string Name,
    string Description,
    string? Notes,
    DateTime Slot,
    string LocationType,
    string Address,
    double? Latitude,
    double? Longitude,
    string Status,
    string PatientId,
    string? DoctorId,
    string? LabPartnerId,
    int? MedicalTestId,
    string? MedicalTestCompletionStatus,
    string CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
