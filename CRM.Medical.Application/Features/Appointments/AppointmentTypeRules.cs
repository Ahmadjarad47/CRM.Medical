using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.AppointmentTypes;

namespace CRM.Medical.Application.Features.Appointments;

internal static class AppointmentTypeRules
{
    public static async Task EnsureActiveTypeAsync(
        IAppointmentTypeRepository appointmentTypes,
        int appointmentTypeId,
        CancellationToken cancellationToken)
    {
        var type = await appointmentTypes.GetByIdAsync(appointmentTypeId, cancellationToken)
            ?? throw new ApplicationBadRequestException("The appointment type was not found.");

        if (!type.IsActive)
            throw new ApplicationBadRequestException("The appointment type is not active.");
    }
}
