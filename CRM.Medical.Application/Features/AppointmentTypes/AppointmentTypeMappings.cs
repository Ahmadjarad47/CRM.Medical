using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.AppointmentTypes;

internal static class AppointmentTypeMappings
{
    public static AppointmentTypeDto ToDto(this AppointmentType t) =>
        new(t.Id, t.Name, t.IsActive, t.CreatedAt);
}
