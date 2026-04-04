namespace CRM.Medical.Application.Features.AppointmentTypes.DTOs;

public sealed record AppointmentTypeDto(int Id, string Name, bool IsActive, DateTime CreatedAt);
