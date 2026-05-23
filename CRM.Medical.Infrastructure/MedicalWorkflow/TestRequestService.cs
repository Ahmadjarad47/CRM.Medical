using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.TestRequests.CQRS;
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
            ["directpatient"] = r => r.DirectPatientId,
            ["medicaltestnamear"] = r => r.MedicalTest.NameAr,
            ["medicaltestnameen"] = r => r.MedicalTest.NameEn,
            ["externalpatientname"] = r => r.ExternalPatient == null ? null : r.ExternalPatient.FullName
        };

    private static readonly IReadOnlyDictionary<string, Func<string, Expression<Func<TestRequest, bool>>?>> ExactSearchFields =
        new Dictionary<string, Func<string, Expression<Func<TestRequest, bool>>?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = token => ParseIntPredicate(token, value => r => r.Id == value),
            ["medicaltestid"] = token => ParseIntPredicate(token, value => r => r.MedicalTestId == value),
            ["externalpatientid"] = token => ParseIntPredicate(token, value => r => r.ExternalPatientId == value)
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
        var textSearchFields = new Dictionary<string, Expression<Func<TestRequest, string?>>>(SearchFields, StringComparer.OrdinalIgnoreCase)
        {
            ["doctorname"] = r => db.Users
                .Where(u => u.Id == r.DoctorId)
                .Select(u => u.FullName)
                .FirstOrDefault(),
            ["labname"] = r => db.Users
                .Where(u => u.Id == r.LabClientId)
                .Select(u => u.FullName)
                .FirstOrDefault(),
            ["patientname"] = r => db.Users
                .Where(u => u.Id == r.DirectPatientId)
                .Select(u => u.FullName)
                .FirstOrDefault()
        };

        query = query.ApplyAdvancedSearch(
            search,
            textSearchFields,
            ExactSearchFields,
            BuildDefaultExactPredicate,
            r => r.Status,
            r => r.Notes,
            r => r.DoctorId,
            r => r.LabClientId,
            r => r.DirectPatientId,
            r => r.MedicalTest.NameAr,
            r => r.MedicalTest.NameEn,
            r => r.ExternalPatient == null ? null : r.ExternalPatient.FullName,
            r => db.Users
                .Where(u => u.Id == r.DoctorId)
                .Select(u => u.FullName)
                .FirstOrDefault(),
            r => db.Users
                .Where(u => u.Id == r.LabClientId)
                .Select(u => u.FullName)
                .FirstOrDefault(),
            r => db.Users
                .Where(u => u.Id == r.DirectPatientId)
                .Select(u => u.FullName)
                .FirstOrDefault());

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

    public async Task<IReadOnlyList<TestRequestDto>> CreateAsync(
        IReadOnlyList<CreateTestRequestItemCommand> items,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        if (items.Count == 0)
            throw new ApplicationBadRequestException("At least one test request item is required.");

        var entities = new List<TestRequest>(items.Count);
        foreach (var item in items)
            entities.Add(await BuildCreateEntityAsync(item, cancellationToken));

        db.TestRequests.AddRange(entities);
        await db.SaveChangesAsync(cancellationToken);

        var medicalTestIds = entities.Select(entity => entity.MedicalTestId).Distinct().ToArray();
        var externalPatientIds = entities
            .Where(entity => entity.ExternalPatientId.HasValue)
            .Select(entity => entity.ExternalPatientId!.Value)
            .Distinct()
            .ToArray();

        var medicalTestNames = await db.MedicalTests
            .AsNoTracking()
            .Where(test => medicalTestIds.Contains(test.Id))
            .ToDictionaryAsync(test => test.Id, test => (string?)test.NameEn, cancellationToken);

        var externalPatientNames = externalPatientIds.Length == 0
            ? new Dictionary<int, string?>()
            : await db.ExternalPatients
                .AsNoTracking()
                .Where(patient => externalPatientIds.Contains(patient.Id))
                .ToDictionaryAsync(patient => patient.Id, patient => (string?)patient.FullName, cancellationToken);

        var userNames = await GetUserNamesByIdsAsync(
            entities.SelectMany(entity => new[] { entity.DoctorId, entity.LabClientId, entity.DirectPatientId }),
            cancellationToken);

        return entities
            .Select(entity => MapCreated(entity, medicalTestNames, externalPatientNames, userNames))
            .ToList();
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

    private async Task<TestRequest> BuildCreateEntityAsync(
        CreateTestRequestItemCommand item,
        CancellationToken cancellationToken)
    {
        var testExists = await db.MedicalTests.AnyAsync(t => t.Id == item.MedicalTestId, cancellationToken);
        if (!testExists)
            throw new ApplicationBadRequestException($"Medical test '{item.MedicalTestId}' was not found.");

        await ValidatePatientSubjectAsync(item.DirectPatientId, item.ExternalPatientId, cancellationToken);

        var entity = new TestRequest
        {
            MedicalTestId = item.MedicalTestId,
            RequestDate = item.RequestDate,
            Status = item.Status.Trim(),
            TotalAmount = item.TotalAmount,
            Notes = string.IsNullOrWhiteSpace(item.Notes) ? null : item.Notes.Trim(),
            Metadata = item.Metadata,
            DoctorId = string.IsNullOrWhiteSpace(item.DoctorId) ? null : item.DoctorId.Trim(),
            LabClientId = string.IsNullOrWhiteSpace(item.LabClientId) ? null : item.LabClientId.Trim(),
            DirectPatientId = string.IsNullOrWhiteSpace(item.DirectPatientId) ? null : item.DirectPatientId.Trim(),
            ExternalPatientId = item.ExternalPatientId,
            CreatedByUserId = currentUser.GetRequiredUserId()
        };

        var canCreate = await accessPolicyEvaluator.CanAccessAsync(entity, "test_requests", "create", cancellationToken);
        if (!canCreate)
            throw new ApplicationForbiddenException("You cannot create this test request.");

        return entity;
    }

    private static TestRequestDto MapCreated(
        TestRequest request,
        IReadOnlyDictionary<int, string?> medicalTestNames,
        IReadOnlyDictionary<int, string?> externalPatientNames,
        IReadOnlyDictionary<string, string> userNames) =>
        new(
            request.Id,
            request.MedicalTestId,
            medicalTestNames.TryGetValue(request.MedicalTestId, out var medicalTestName) ? medicalTestName : null,
            request.DoctorId,
            ResolveUserName(userNames, request.DoctorId),
            request.LabClientId,
            ResolveUserName(userNames, request.LabClientId),
            request.DirectPatientId,
            ResolveUserName(userNames, request.DirectPatientId)
                ?? (request.ExternalPatientId.HasValue &&
                    externalPatientNames.TryGetValue(request.ExternalPatientId.Value, out var externalPatientName)
                    ? externalPatientName
                    : null),
            request.ExternalPatientId,
            request.ExternalPatientId.HasValue &&
            externalPatientNames.TryGetValue(request.ExternalPatientId.Value, out var fullName)
                ? fullName
                : null,
            request.RequestDate,
            request.Status,
            request.TotalAmount,
            request.Notes,
            MedicalWorkflowJson.ToJsonElement(request.Metadata),
            request.CreatedAt,
            request.UpdatedAt);

    private static Expression<Func<TestRequest, bool>>? BuildDefaultExactPredicate(string token) =>
        ParseIntPredicate(token, value => r =>
            r.Id == value ||
            r.MedicalTestId == value ||
            r.ExternalPatientId == value);

    private static Expression<Func<TestRequest, bool>>? ParseIntPredicate(
        string token,
        Func<int, Expression<Func<TestRequest, bool>>> predicateFactory) =>
        int.TryParse(token, out var value) ? predicateFactory(value) : null;
}
