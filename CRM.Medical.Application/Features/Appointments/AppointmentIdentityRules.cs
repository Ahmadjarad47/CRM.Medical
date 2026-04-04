using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Appointments;

internal static class AppointmentIdentityRules
{
    public static async Task EnsureUserHasRoleAsync(
        UserManager<User> userManager,
        string userId,
        string roleName,
        string roleDescriptionForMessage,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ApplicationNotFoundException($"User '{userId}' was not found.");

        if (!await userManager.IsInRoleAsync(user, roleName))
            throw new ApplicationBadRequestException($"The user is not registered as {roleDescriptionForMessage}.");
    }

    public static async Task EnsurePatientAsync(
        UserManager<User> userManager,
        string patientId,
        CancellationToken cancellationToken) =>
        await EnsureUserHasRoleAsync(
            userManager,
            patientId,
            UserRoles.Patient,
            "a patient",
            cancellationToken);

    public static async Task EnsureDoctorAsync(
        UserManager<User> userManager,
        string doctorId,
        CancellationToken cancellationToken) =>
        await EnsureUserHasRoleAsync(
            userManager,
            doctorId,
            UserRoles.Doctor,
            "a doctor",
            cancellationToken);

    public static async Task EnsureLabPartnerAsync(
        UserManager<User> userManager,
        string labId,
        CancellationToken cancellationToken) =>
        await EnsureUserHasRoleAsync(
            userManager,
            labId,
            UserRoles.LabPartner,
            "a lab partner",
            cancellationToken);
}
