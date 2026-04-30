using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

/// <summary>
/// Role-based row access for <see cref="TestRequest"/> (permission checks are applied separately).
/// </summary>
internal sealed class TestRequestAccessEvaluator(MedicalDbContext db, ICurrentUserAccessor user)
{
    public async Task EnsureCanAccessAsync(TestRequest request, CancellationToken cancellationToken)
    {
        var userId = user.UserId
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        if (user.IsInRole(UserRoles.Admin))
            return;

        if (user.IsInRole(UserRoles.Patient))
        {
            if (request.DirectPatientId == userId)
                return;

            if (request.ExternalPatientId is not null
                && await db.ExternalPatients.AsNoTracking().AnyAsync(
                    e => e.Id == request.ExternalPatientId && e.LinkedDirectPatientId == userId,
                    cancellationToken))
                return;

            throw new ApplicationForbiddenException("You cannot access this test request.");
        }

        if (user.IsInRole(UserRoles.LabPartner))
        {
            if (request.LabClientId == userId || request.CreatedByUserId == userId)
                return;
            throw new ApplicationForbiddenException("You cannot access this test request.");
        }

        if (user.IsInRole(UserRoles.Doctor))
        {
            if (request.DoctorId == userId || request.CreatedByUserId == userId)
                return;

            if (request.DirectPatientId is not null)
            {
                var linked = await db.Users.AsNoTracking().AnyAsync(
                    u => u.Id == request.DirectPatientId && u.CreatedByUserId == userId,
                    cancellationToken);
                if (linked)
                    return;
            }

            if (request.ExternalPatientId is not null)
            {
                var linkedExternalPatient = await (
                    from e in db.ExternalPatients.AsNoTracking()
                    join u in db.Users.AsNoTracking() on e.LinkedDirectPatientId equals u.Id
                    where e.Id == request.ExternalPatientId && u.CreatedByUserId == userId
                    select e.Id
                ).AnyAsync(cancellationToken);
                if (linkedExternalPatient)
                    return;
            }

            throw new ApplicationForbiddenException("You cannot access this test request.");
        }

        if (string.Equals(request.CreatedByUserId, userId, StringComparison.Ordinal))
            return;

        throw new ApplicationForbiddenException("You cannot access this test request.");
    }

    public IQueryable<TestRequest> FilterAccessible(IQueryable<TestRequest> query)
    {
        var userId = user.UserId;
        if (string.IsNullOrEmpty(userId))
            return query.Where(_ => false);

        if (user.IsInRole(UserRoles.Admin))
            return query;

        if (user.IsInRole(UserRoles.Patient))
        {
            return query.Where(r =>
                r.DirectPatientId == userId
                || (
                    r.ExternalPatientId != null
                    && db.ExternalPatients.Any(
                        e =>
                            e.Id == r.ExternalPatientId
                            && e.LinkedDirectPatientId == userId)));
        }

        if (user.IsInRole(UserRoles.LabPartner))
            return query.Where(r => r.LabClientId == userId || r.CreatedByUserId == userId);

        if (user.IsInRole(UserRoles.Doctor))
        {
            var patientIds = db.Users.AsNoTracking()
                .Where(u => u.CreatedByUserId == userId)
                .Select(u => u.Id);

            return query.Where(r =>
                r.DoctorId == userId
                || r.CreatedByUserId == userId
                || (r.DirectPatientId != null && patientIds.Contains(r.DirectPatientId))
                || (
                    r.ExternalPatientId != null
                    && db.ExternalPatients.Any(
                        e =>
                            e.Id == r.ExternalPatientId
                            && e.LinkedDirectPatientId != null
                            && patientIds.Contains(e.LinkedDirectPatientId))));
        }

        return query.Where(r => r.CreatedByUserId == userId);
    }
}
