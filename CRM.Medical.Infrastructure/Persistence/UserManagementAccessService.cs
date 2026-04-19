using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class UserManagementAccessService(
    UserManager<User> userManager,
    MedicalDbContext db)
    : IUserManagementAccess
{
    public async Task EnsureActorCanCreateUsersAsync(
        string actorUserId,
        CancellationToken cancellationToken = default)
    {
        var actor = await userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        if (await userManager.IsInRoleAsync(actor, UserRoles.Admin))
            return;

        if (await userManager.IsInRoleAsync(actor, UserRoles.Doctor))
            return;

        if (await userManager.IsInRoleAsync(actor, UserRoles.LabPartner))
            return;

        throw new ApplicationForbiddenException(
            "Only administrators, doctors, and lab partners can create users.");
    }

    public async Task EnsureActorCanManageUserAsync(
        string actorUserId,
        User targetUser,
        CancellationToken cancellationToken = default)
    {
        if (string.Equals(actorUserId, targetUser.Id, StringComparison.Ordinal))
            return;

        var actor = await userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        if (await userManager.IsInRoleAsync(actor, UserRoles.Admin))
            return;

        if (await userManager.IsInRoleAsync(actor, UserRoles.Doctor))
        {
            if (IsCreatedBy(actorUserId, targetUser))
                return;

            if (await HasDoctorPatientAppointmentAsync(actorUserId, targetUser.Id, cancellationToken))
                return;

            throw new ApplicationForbiddenException(
                "You can only manage users you created or patients who have appointments with you.");
        }

        if (await userManager.IsInRoleAsync(actor, UserRoles.LabPartner))
        {
            if (IsCreatedBy(actorUserId, targetUser))
                return;

            if (await HasLabPatientAppointmentAsync(actorUserId, targetUser.Id, cancellationToken))
                return;

            throw new ApplicationForbiddenException(
                "You can only manage users you created or patients who have appointments with your lab.");
        }

        throw new ApplicationForbiddenException("You are not allowed to manage this user.");
    }

    public async Task<IQueryable<User>> ScopeUsersQueryForActorAsync(
        IQueryable<User> users,
        string actorUserId,
        CancellationToken cancellationToken = default)
    {
        var actor = await userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        if (await userManager.IsInRoleAsync(actor, UserRoles.Admin))
            return users;

        if (await userManager.IsInRoleAsync(actor, UserRoles.Doctor))
        {
            return users.Where(u =>
                u.CreatedByUserId == actorUserId ||
                db.Appointments.Any(a => a.DoctorId == actorUserId && a.PatientId == u.Id));
        }

        if (await userManager.IsInRoleAsync(actor, UserRoles.LabPartner))
        {
            return users.Where(u =>
                u.CreatedByUserId == actorUserId ||
                db.Appointments.Any(a => a.LabPartnerId == actorUserId && a.PatientId == u.Id));
        }

        throw new ApplicationForbiddenException("You are not allowed to list users.");
    }

    private static bool IsCreatedBy(string actorUserId, User targetUser) =>
        string.Equals(targetUser.CreatedByUserId, actorUserId, StringComparison.Ordinal);

    private Task<bool> HasDoctorPatientAppointmentAsync(
        string doctorId,
        string patientId,
        CancellationToken cancellationToken) =>
        db.Appointments.AsNoTracking()
            .AnyAsync(
                a => a.DoctorId == doctorId && a.PatientId == patientId,
                cancellationToken);

    private Task<bool> HasLabPatientAppointmentAsync(
        string labPartnerId,
        string patientId,
        CancellationToken cancellationToken) =>
        db.Appointments.AsNoTracking()
            .AnyAsync(
                a => a.LabPartnerId == labPartnerId && a.PatientId == patientId,
                cancellationToken);
}
