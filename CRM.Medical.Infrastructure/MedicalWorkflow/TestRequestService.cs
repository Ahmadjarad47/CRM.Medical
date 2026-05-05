using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Services;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class TestRequestService(
    MedicalDbContext db,
    ICurrentUserAccessor currentUser,
    UserManager<User> userManager) : ITestRequestService
{
    private readonly TestRequestAccessEvaluator _access = new(db, currentUser);
    private static readonly IReadOnlyDictionary<string, Expression<Func<TestRequest, string?>>> SearchFields =
        new Dictionary<string, Expression<Func<TestRequest, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["status"] = r => r.Status,
            ["notes"] = r => r.Notes,
            ["doctor"] = r => r.DoctorId,
            ["lab"] = r => r.LabClientId,
            ["directpatient"] = r => r.DirectPatientId
        };

    public async Task<PagedResult<TestRequestDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestRequestRead);

        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = _access.FilterAccessible(db.TestRequests.AsNoTracking());

        query = query.ApplyAdvancedSearch(search, SearchFields, r => r.Status, r => r.Notes, r => r.DoctorId, r => r.LabClientId, r => r.DirectPatientId);

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .Include(r => r.MedicalTest)
            .Include(r => r.ExternalPatient)
            .OrderByDescending(r => r.RequestDate)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TestRequestDto>
        {
            Items = rows.Select(Map).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TestRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestRequestRead);

        var entity = await db.TestRequests
            .AsNoTracking()
            .Include(r => r.MedicalTest)
            .Include(r => r.ExternalPatient)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{id}' was not found.");

        await _access.EnsureCanAccessAsync(entity, cancellationToken);
        return Map(entity);
    }

    public async Task<TestRequestDto> CreateAsync(
        int medicalTestId,
        DateTime requestDate,
        string status,
        double totalAmount,
        string? notes,
        JsonDocument? metadata,
        string? doctorId,
        string? labClientId,
        string? directPatientId,
        int? externalPatientId,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestRequestCreate);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var testExists = await db.MedicalTests.AnyAsync(t => t.Id == medicalTestId, cancellationToken);
        if (!testExists)
            throw new ApplicationBadRequestException($"Medical test '{medicalTestId}' was not found.");

        await ValidatePatientSubjectAsync(directPatientId, externalPatientId, cancellationToken);

        var userId = currentUser.GetRequiredUserId();
        var isAdmin = currentUser.IsInRole(UserRoles.Admin);

        string? resolvedDoctorId;
        string? resolvedLabId;

        if (isAdmin)
        {
            resolvedDoctorId = string.IsNullOrWhiteSpace(doctorId) ? null : doctorId.Trim();
            resolvedLabId = string.IsNullOrWhiteSpace(labClientId) ? null : labClientId.Trim();
        }
        else if (currentUser.IsInRole(UserRoles.Doctor))
        {
            resolvedDoctorId = userId;
            resolvedLabId = string.IsNullOrWhiteSpace(labClientId) ? null : labClientId.Trim();
        }
        else if (currentUser.IsInRole(UserRoles.LabPartner))
        {
            resolvedLabId = userId;
            resolvedDoctorId = string.IsNullOrWhiteSpace(doctorId) ? null : doctorId.Trim();
        }
        else
        {
            resolvedDoctorId = string.IsNullOrWhiteSpace(doctorId) ? null : doctorId.Trim();
            resolvedLabId = string.IsNullOrWhiteSpace(labClientId) ? null : labClientId.Trim();
        }

        var entity = new TestRequest
        {
            MedicalTestId = medicalTestId,
            RequestDate = requestDate,
            Status = status.Trim(),
            TotalAmount = totalAmount,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Metadata = metadata,
            DoctorId = resolvedDoctorId,
            LabClientId = resolvedLabId,
            DirectPatientId = string.IsNullOrWhiteSpace(directPatientId) ? null : directPatientId.Trim(),
            ExternalPatientId = externalPatientId,
            CreatedByUserId = userId
        };

        db.TestRequests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(entity).Reference(r => r.MedicalTest).LoadAsync(cancellationToken);
        await db.Entry(entity).Reference(r => r.ExternalPatient).LoadAsync(cancellationToken);
        return Map(entity);
    }

    public async Task UpdateAsync(
        int id,
        DateTime requestDate,
        string status,
        double totalAmount,
        string? notes,
        JsonDocument? metadata,
        string? doctorId,
        string? labClientId,
        string? directPatientId,
        int? externalPatientId,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestRequestUpdate);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var entity = await db.TestRequests
            .Include(r => r.MedicalTest)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{id}' was not found.");

        await _access.EnsureCanAccessAsync(entity, cancellationToken);

        await ValidatePatientSubjectAsync(
            string.IsNullOrWhiteSpace(directPatientId) ? null : directPatientId.Trim(),
            externalPatientId,
            cancellationToken);

        var userId = currentUser.GetRequiredUserId();
        var isAdmin = currentUser.IsInRole(UserRoles.Admin);

        if (isAdmin)
        {
            entity.DoctorId = string.IsNullOrWhiteSpace(doctorId) ? null : doctorId.Trim();
            entity.LabClientId = string.IsNullOrWhiteSpace(labClientId) ? null : labClientId.Trim();
        }
        else if (currentUser.IsInRole(UserRoles.Doctor))
        {
            entity.DoctorId = userId;
            if (!string.IsNullOrWhiteSpace(labClientId))
                entity.LabClientId = labClientId.Trim();
        }
        else if (currentUser.IsInRole(UserRoles.LabPartner))
        {
            entity.LabClientId = userId;
            if (!string.IsNullOrWhiteSpace(doctorId))
                entity.DoctorId = doctorId.Trim();
        }

        entity.DirectPatientId = string.IsNullOrWhiteSpace(directPatientId) ? null : directPatientId.Trim();
        entity.ExternalPatientId = externalPatientId;
        entity.RequestDate = requestDate;
        entity.Status = status.Trim();
        entity.TotalAmount = totalAmount;
        entity.Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        entity.Metadata = metadata;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestRequestDelete);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var entity = await db.TestRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{id}' was not found.");

        await _access.EnsureCanAccessAsync(entity, cancellationToken);

        db.TestRequests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidatePatientSubjectAsync(string? directPatientId, int? externalPatientId, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(directPatientId) && externalPatientId.HasValue)
            throw new ApplicationBadRequestException("Specify either DirectPatientId or ExternalPatientId, not both.");

        await ValidateDirectPatientAsync(
            directPatientId is null ? null : directPatientId.Trim(),
            cancellationToken);

        await ValidateExternalPatientAsync(externalPatientId, cancellationToken);
    }

    private async Task ValidateDirectPatientAsync(string? directPatientId, CancellationToken cancellationToken)
    {
        if (directPatientId is null)
            return;

        var patient = await userManager.FindByIdAsync(directPatientId);
        if (patient is null)
            throw new ApplicationBadRequestException("Direct patient was not found.");

        if (!await userManager.IsInRoleAsync(patient, UserRoles.Patient))
            throw new ApplicationBadRequestException("DirectPatientId must reference a patient account.");

        if (currentUser.IsInRole(UserRoles.Admin) || currentUser.IsInRole(UserRoles.LabPartner))
            return;

        if (currentUser.IsInRole(UserRoles.Doctor))
        {
            var actorId = currentUser.GetRequiredUserId();
            if (!string.Equals(patient.CreatedByUserId, actorId, StringComparison.Ordinal))
                throw new ApplicationForbiddenException("You may only assign patients under your care.");
        }
    }

    private async Task ValidateExternalPatientAsync(int? externalPatientId, CancellationToken cancellationToken)
    {
        if (!externalPatientId.HasValue)
            return;

        var exists = await db.ExternalPatients.AnyAsync(e => e.Id == externalPatientId.Value, cancellationToken);
        if (!exists)
            throw new ApplicationBadRequestException($"External patient '{externalPatientId.Value}' was not found.");
    }

    private static TestRequestDto Map(TestRequest r) =>
        new(
            r.Id,
            r.MedicalTestId,
            r.MedicalTest?.NameEn,
            r.DoctorId,
            r.LabClientId,
            r.DirectPatientId,
            r.ExternalPatientId,
            r.ExternalPatient?.FullName,
            r.RequestDate,
            r.Status,
            r.TotalAmount,
            r.Notes,
            MedicalWorkflowJson.ToJsonElement(r.Metadata),
            r.CreatedAt,
            r.UpdatedAt);
}
