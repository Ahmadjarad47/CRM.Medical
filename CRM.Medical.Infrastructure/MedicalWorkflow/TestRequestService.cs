using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Services;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class TestRequestService(
    MedicalDbContext db,
    ICurrentUserAccessor currentUser,
    UserManager<User> userManager) : ITestRequestService
{
    private readonly TestRequestAccessEvaluator _access = new(db, currentUser);

    public async Task<IReadOnlyList<TestRequestDto>> ListAsync(CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestRequestRead);

        var query = _access.FilterAccessible(db.TestRequests.AsNoTracking());
        var rows = await query
            .Include(r => r.MedicalTest)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync(cancellationToken);

        return rows.Select(Map).ToList();
    }

    public async Task<TestRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestRequestRead);

        var entity = await db.TestRequests
            .AsNoTracking()
            .Include(r => r.MedicalTest)
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
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestRequestCreate);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var testExists = await db.MedicalTests.AnyAsync(t => t.Id == medicalTestId, cancellationToken);
        if (!testExists)
            throw new ApplicationBadRequestException($"Medical test '{medicalTestId}' was not found.");

        await ValidateDirectPatientAsync(directPatientId, cancellationToken);

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
            CreatedByUserId = userId
        };

        db.TestRequests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(entity).Reference(r => r.MedicalTest).LoadAsync(cancellationToken);
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

        await ValidateDirectPatientAsync(
            string.IsNullOrWhiteSpace(directPatientId) ? null : directPatientId.Trim(),
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

    private static TestRequestDto Map(TestRequest r) =>
        new(
            r.Id,
            r.MedicalTestId,
            r.MedicalTest?.NameEn,
            r.DoctorId,
            r.LabClientId,
            r.DirectPatientId,
            r.RequestDate,
            r.Status,
            r.TotalAmount,
            r.Notes,
            MedicalWorkflowJson.ToJsonElement(r.Metadata),
            r.CreatedAt,
            r.UpdatedAt);
}
