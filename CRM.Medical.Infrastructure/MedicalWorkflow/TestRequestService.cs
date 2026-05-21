using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Services;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class TestRequestService(
    MedicalDbContext db,
    ICurrentUserAccessor currentUser,
    UserManager<User> userManager,
    IAccessPolicyEvaluator accessPolicyEvaluator) : ITestRequestService
{
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

        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = await accessPolicyEvaluator.ApplyAsync(db.TestRequests.AsNoTracking(), "test_requests", "read", cancellationToken);

        query = query.ApplyAdvancedSearch(search, SearchFields, r => r.Status, r => r.Notes, r => r.DoctorId, r => r.LabClientId, r => r.DirectPatientId);

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .Include(r => r.MedicalTest)
            .Include(r => r.ExternalPatient)
            .OrderByDescending(r => r.RequestDate)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        var userNames = await GetUserNamesByIdsAsync(
            rows.SelectMany(row => new[] { row.DoctorId, row.LabClientId, row.DirectPatientId }),
            cancellationToken);

        return new PagedResult<TestRequestDto>
        {
            Items = rows.Select(row => Map(row, userNames)).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TestRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var entity = await db.TestRequests
            .AsNoTracking()
            .Include(r => r.MedicalTest)
            .Include(r => r.ExternalPatient)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{id}' was not found.");

        var canAccess = await accessPolicyEvaluator.CanAccessAsync(entity, "test_requests", "read", cancellationToken);
        if (!canAccess)
            throw new ApplicationForbiddenException("You cannot access this test request.");

        var userNames = await GetUserNamesByIdsAsync([entity.DoctorId, entity.LabClientId, entity.DirectPatientId], cancellationToken);
        return Map(entity, userNames);
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

        var testExists = await db.MedicalTests.AnyAsync(t => t.Id == medicalTestId, cancellationToken);
        if (!testExists)
            throw new ApplicationBadRequestException($"Medical test '{medicalTestId}' was not found.");

        await ValidatePatientSubjectAsync(directPatientId, externalPatientId, cancellationToken);

        var userId = currentUser.GetRequiredUserId();
        var resolvedDoctorId = string.IsNullOrWhiteSpace(doctorId) ? null : doctorId.Trim();
        var resolvedLabId = string.IsNullOrWhiteSpace(labClientId) ? null : labClientId.Trim();

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
        var canCreate = await accessPolicyEvaluator.CanAccessAsync(entity, "test_requests", "create", cancellationToken);
        if (!canCreate)
            throw new ApplicationForbiddenException("You cannot create this test request.");

        db.TestRequests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(entity).Reference(r => r.MedicalTest).LoadAsync(cancellationToken);
        await db.Entry(entity).Reference(r => r.ExternalPatient).LoadAsync(cancellationToken);
        var userNames = await GetUserNamesByIdsAsync([entity.DoctorId, entity.LabClientId, entity.DirectPatientId], cancellationToken);
        return Map(entity, userNames);
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
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var entity = await db.TestRequests
            .Include(r => r.MedicalTest)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{id}' was not found.");

        var canUpdate = await accessPolicyEvaluator.CanAccessAsync(entity, "test_requests", "update", cancellationToken);
        if (!canUpdate)
            throw new ApplicationForbiddenException("You cannot modify this test request.");

        await ValidatePatientSubjectAsync(
            string.IsNullOrWhiteSpace(directPatientId) ? null : directPatientId.Trim(),
            externalPatientId,
            cancellationToken);

        var userId = currentUser.GetRequiredUserId();
        entity.DoctorId = string.IsNullOrWhiteSpace(doctorId) ? null : doctorId.Trim();
        entity.LabClientId = string.IsNullOrWhiteSpace(labClientId) ? null : labClientId.Trim();

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
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var entity = await db.TestRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{id}' was not found.");

        var canDelete = await accessPolicyEvaluator.CanAccessAsync(entity, "test_requests", "delete", cancellationToken);
        if (!canDelete)
            throw new ApplicationForbiddenException("You cannot delete this test request.");

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

        if (!await userManager.IsInRoleAsync(patient, "Patient"))
            throw new ApplicationBadRequestException("DirectPatientId must reference a patient account.");
    }

    private async Task ValidateExternalPatientAsync(int? externalPatientId, CancellationToken cancellationToken)
    {
        if (!externalPatientId.HasValue)
            return;

        var exists = await db.ExternalPatients.AnyAsync(e => e.Id == externalPatientId.Value, cancellationToken);
        if (!exists)
            throw new ApplicationBadRequestException($"External patient '{externalPatientId.Value}' was not found.");
    }

    private async Task<Dictionary<string, string>> GetUserNamesByIdsAsync(
        IEnumerable<string?> userIds,
        CancellationToken cancellationToken)
    {
        var ids = userIds
            .Where(userId => !string.IsNullOrWhiteSpace(userId))
            .Select(userId => userId!.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (ids.Length == 0)
            return new Dictionary<string, string>(StringComparer.Ordinal);

        return await db.Users
            .AsNoTracking()
            .Where(user => ids.Contains(user.Id))
            .ToDictionaryAsync(user => user.Id, user => user.FullName, StringComparer.Ordinal, cancellationToken);
    }

    private static TestRequestDto Map(TestRequest r, IReadOnlyDictionary<string, string> userNames) =>
        new(
            r.Id,
            r.MedicalTestId,
            r.MedicalTest?.NameEn,
            r.DoctorId,
            ResolveUserName(userNames, r.DoctorId),
            r.LabClientId,
            ResolveUserName(userNames, r.LabClientId),
            r.DirectPatientId,
            ResolvePatientName(r, userNames),
            r.ExternalPatientId,
            r.ExternalPatient?.FullName,
            r.RequestDate,
            r.Status,
            r.TotalAmount,
            r.Notes,
            MedicalWorkflowJson.ToJsonElement(r.Metadata),
            r.CreatedAt,
            r.UpdatedAt);

    private static string? ResolveUserName(IReadOnlyDictionary<string, string> userNames, string? userId) =>
        !string.IsNullOrWhiteSpace(userId) && userNames.TryGetValue(userId, out var fullName)
            ? fullName
            : null;

    private static string? ResolvePatientName(TestRequest request, IReadOnlyDictionary<string, string> userNames) =>
        ResolveUserName(userNames, request.DirectPatientId) ?? request.ExternalPatient?.FullName;
}
