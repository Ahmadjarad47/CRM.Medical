using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using CRM.Medical.Application.Features.ExternalPatients.Services;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class ExternalPatientService(
    MedicalDbContext db,
    ICurrentUserAccessor currentUser,
    UserManager<User> userManager) : IExternalPatientService
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<ExternalPatient, string?>>> SearchFields =
        new Dictionary<string, Expression<Func<ExternalPatient, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = e => e.FullName,
            ["phone"] = e => e.PhoneNumber,
            ["externalid"] = e => e.ExternalId,
            ["gender"] = e => e.Gender
        };

    private readonly TestRequestAccessEvaluator _access = new(db, currentUser);

    public async Task<PagedResult<ExternalPatientDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.ExternalPatientsManage);

        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = FilterAccessible(db.ExternalPatients.AsNoTracking());

        query = query.ApplyAdvancedSearch(search, SearchFields, e => e.FullName, e => e.PhoneNumber, e => e.ExternalId);

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(e => e.CreatedAt)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ExternalPatientDto>
        {
            Items = rows.Select(Map).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ExternalPatientDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.ExternalPatientsManage);

        var entity = await FilterAccessible(db.ExternalPatients.AsNoTracking())
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"External patient '{id}' was not found.");

        return Map(entity);
    }

    public async Task<ExternalPatientDto> CreateAsync(
        string fullName,
        int? age,
        string gender,
        string phoneNumber,
        string? externalId,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.ExternalPatientsManage);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ApplicationBadRequestException("Full name is required.");
        if (string.IsNullOrWhiteSpace(gender))
            throw new ApplicationBadRequestException("Gender is required.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ApplicationBadRequestException("Phone number is required.");

        var entity = new ExternalPatient
        {
            FullName = fullName.Trim(),
            Age = age,
            Gender = gender.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim(),
            CreatedByUserId = currentUser.GetRequiredUserId()
        };

        db.ExternalPatients.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task LinkToDirectPatientAsync(int externalPatientId, string directPatientUserId, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.ExternalPatientsManage);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        if (string.IsNullOrWhiteSpace(directPatientUserId))
            throw new ApplicationBadRequestException("DirectPatientUserId is required.");

        directPatientUserId = directPatientUserId.Trim();

        var patient = await userManager.FindByIdAsync(directPatientUserId)
            ?? throw new ApplicationBadRequestException("Direct patient was not found.");

        if (!await userManager.IsInRoleAsync(patient, UserRoles.Patient))
            throw new ApplicationBadRequestException("The user must be a patient account.");

        if (currentUser.IsInRole(UserRoles.Doctor))
        {
            var doctorId = currentUser.GetRequiredUserId();
            if (!string.Equals(patient.CreatedByUserId, doctorId, StringComparison.Ordinal))
                throw new ApplicationForbiddenException("You may only link patients under your care.");
        }

        var entity = await db.ExternalPatients.FirstOrDefaultAsync(e => e.Id == externalPatientId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"External patient '{externalPatientId}' was not found.");

        entity.LinkToDirectPatient(directPatientUserId);
        await db.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<ExternalPatient> FilterAccessible(IQueryable<ExternalPatient> query)
    {
        var userId = currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return query.Where(_ => false);

        if (currentUser.IsInRole(UserRoles.Admin))
            return query;

        var fromRequests = _access
            .FilterAccessible(db.TestRequests.AsNoTracking())
            .Where(r => r.ExternalPatientId != null)
            .Select(r => r.ExternalPatientId!.Value);

        if (currentUser.IsInRole(UserRoles.Patient))
        {
            return query.Where(e =>
                e.LinkedDirectPatientId == userId
                || fromRequests.Contains(e.Id));
        }

        if (currentUser.IsInRole(UserRoles.LabPartner))
        {
            return query.Where(e =>
                e.CreatedByUserId == userId
                || fromRequests.Contains(e.Id));
        }

        if (currentUser.IsInRole(UserRoles.Doctor))
        {
            var patientIds = db.Users.AsNoTracking()
                .Where(u => u.CreatedByUserId == userId)
                .Select(u => u.Id);

            return query.Where(e =>
                e.CreatedByUserId == userId
                || fromRequests.Contains(e.Id)
                || (e.LinkedDirectPatientId != null && patientIds.Contains(e.LinkedDirectPatientId)));
        }

        return query.Where(e =>
            e.CreatedByUserId == userId
            || fromRequests.Contains(e.Id));
    }

    private static ExternalPatientDto Map(ExternalPatient e) =>
        new(e.Id, e.FullName, e.Age, e.Gender, e.PhoneNumber, e.ExternalId, e.LinkedDirectPatientId, e.CreatedAt);
}
