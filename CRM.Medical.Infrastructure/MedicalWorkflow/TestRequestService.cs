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

    public async Task<PagedResult<GroupedTestRequestDto>> ListAsync(
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

        var rows = await query
            .Include(r => r.MedicalTest)
            .Include(r => r.ExternalPatient)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync(cancellationToken);

        var userNames = await GetUserNamesByIdsAsync(
            rows.SelectMany(row => new[] { row.DoctorId, row.LabClientId, row.DirectPatientId }),
            cancellationToken);

        var groupedRows = rows
            .GroupBy(BuildGroupKey)
            .OrderByDescending(group => group.Max(row => row.RequestDate))
            .ThenByDescending(group => group.Key.CreatedAt)
            .Select(group => MapGroup(group, userNames))
            .ToList();

        var totalCount = groupedRows.Count;
        var pagedItems = groupedRows
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToList();

        return new PagedResult<GroupedTestRequestDto>
        {
            Items = pagedItems,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    public async Task<GroupedTestRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken)
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

        var groupedEntities = await db.TestRequests
            .AsNoTracking()
            .Include(r => r.MedicalTest)
            .Include(r => r.ExternalPatient)
            .Where(r =>
                r.CreatedAt == entity.CreatedAt &&
                r.CreatedByUserId == entity.CreatedByUserId &&
                r.RequestDate == entity.RequestDate &&
                r.Status == entity.Status &&
                r.TotalAmount == entity.TotalAmount &&
                r.Notes == entity.Notes &&
                r.DoctorId == entity.DoctorId &&
                r.LabClientId == entity.LabClientId &&
                r.DirectPatientId == entity.DirectPatientId &&
                r.ExternalPatientId == entity.ExternalPatientId)
            .ToListAsync(cancellationToken);

        var userNames = await GetUserNamesByIdsAsync(
            groupedEntities.SelectMany(row => new[] { row.DoctorId, row.LabClientId, row.DirectPatientId }),
            cancellationToken);

        return MapGroup(groupedEntities, userNames);
    }

    public async Task<GroupedTestRequestDto> CreateAsync(
        IReadOnlyList<int> medicalTestIds,
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
        if (medicalTestIds.Count == 0)
            throw new ApplicationBadRequestException("At least one medicalTestId is required.");

        await ValidatePatientSubjectAsync(directPatientId, externalPatientId, cancellationToken);

        var entities = new List<TestRequest>(medicalTestIds.Count);
        foreach (var medicalTestId in medicalTestIds.Distinct())
            entities.Add(await BuildCreateEntityAsync(
                medicalTestId,
                requestDate,
                status,
                totalAmount,
                notes,
                metadata,
                doctorId,
                labClientId,
                directPatientId,
                externalPatientId,
                cancellationToken));

        db.TestRequests.AddRange(entities);
        await db.SaveChangesAsync(cancellationToken);

        var createdMedicalTestIds = entities.Select(entity => entity.MedicalTestId).Distinct().ToArray();
        var externalPatientIds = entities
            .Where(entity => entity.ExternalPatientId.HasValue)
            .Select(entity => entity.ExternalPatientId!.Value)
            .Distinct()
            .ToArray();

        var medicalTests = await db.MedicalTests
            .AsNoTracking()
            .Where(test => createdMedicalTestIds.Contains(test.Id))
            .ToDictionaryAsync(
                test => test.Id,
                test => new MedicalTestLookupItem(test.NameEn, test.ParameterSchema),
                cancellationToken);

        var externalPatientNames = externalPatientIds.Length == 0
            ? new Dictionary<int, string?>()
            : await db.ExternalPatients
                .AsNoTracking()
                .Where(patient => externalPatientIds.Contains(patient.Id))
                .ToDictionaryAsync(patient => patient.Id, patient => (string?)patient.FullName, cancellationToken);

        var userNames = await GetUserNamesByIdsAsync(
            entities.SelectMany(entity => new[] { entity.DoctorId, entity.LabClientId, entity.DirectPatientId }),
            cancellationToken);

        return MapCreatedGroup(entities, medicalTests, externalPatientNames, userNames);
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

    private static string? ResolveUserName(IReadOnlyDictionary<string, string> userNames, string? userId) =>
        !string.IsNullOrWhiteSpace(userId) && userNames.TryGetValue(userId, out var fullName)
            ? fullName
            : null;

    private static string? ResolvePatientName(TestRequest request, IReadOnlyDictionary<string, string> userNames) =>
        ResolveUserName(userNames, request.DirectPatientId) ?? request.ExternalPatient?.FullName;

    private async Task<TestRequest> BuildCreateEntityAsync(
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
        var testExists = await db.MedicalTests.AnyAsync(t => t.Id == medicalTestId, cancellationToken);
        if (!testExists)
            throw new ApplicationBadRequestException($"Medical test '{medicalTestId}' was not found.");

        var entity = new TestRequest
        {
            MedicalTestId = medicalTestId,
            RequestDate = requestDate,
            Status = status.Trim(),
            TotalAmount = totalAmount,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Metadata = metadata,
            DoctorId = string.IsNullOrWhiteSpace(doctorId) ? null : doctorId.Trim(),
            LabClientId = string.IsNullOrWhiteSpace(labClientId) ? null : labClientId.Trim(),
            DirectPatientId = string.IsNullOrWhiteSpace(directPatientId) ? null : directPatientId.Trim(),
            ExternalPatientId = externalPatientId,
            CreatedByUserId = currentUser.GetRequiredUserId()
        };

        var canCreate = await accessPolicyEvaluator.CanAccessAsync(entity, "test_requests", "create", cancellationToken);
        if (!canCreate)
            throw new ApplicationForbiddenException("You cannot create this test request.");

        return entity;
    }

    private static GroupedTestRequestDto MapCreatedGroup(
        IReadOnlyList<TestRequest> requests,
        IReadOnlyDictionary<int, MedicalTestLookupItem> medicalTests,
        IReadOnlyDictionary<int, string?> externalPatientNames,
        IReadOnlyDictionary<string, string> userNames)
    {
        var primaryRequest = requests
            .OrderBy(request => request.Id)
            .First();

        var tests = requests
            .OrderBy(request => request.Id)
            .Select(request => new TestRequestMedicalTestItemDto(
                request.Id,
                request.MedicalTestId,
                medicalTests.TryGetValue(request.MedicalTestId, out var medicalTest) ? medicalTest.NameEn : null,
                BuildParameterItems(
                    medicalTests.TryGetValue(request.MedicalTestId, out medicalTest) ? medicalTest.ParameterSchema : null,
                    request.Metadata)))
            .ToList();

        return new(
            primaryRequest.Id,
            tests.Select(test => test.TestRequestId).ToList(),
            tests,
            primaryRequest.DoctorId,
            ResolveUserName(userNames, primaryRequest.DoctorId),
            primaryRequest.LabClientId,
            ResolveUserName(userNames, primaryRequest.LabClientId),
            primaryRequest.DirectPatientId,
            ResolveUserName(userNames, primaryRequest.DirectPatientId)
                ?? (primaryRequest.ExternalPatientId.HasValue &&
                    externalPatientNames.TryGetValue(primaryRequest.ExternalPatientId.Value, out var externalPatientName)
                    ? externalPatientName
                    : null),
            primaryRequest.ExternalPatientId,
            primaryRequest.ExternalPatientId.HasValue &&
            externalPatientNames.TryGetValue(primaryRequest.ExternalPatientId.Value, out var fullName)
                ? fullName
                : null,
            primaryRequest.RequestDate,
            primaryRequest.Status,
            primaryRequest.TotalAmount,
            primaryRequest.Notes,
            MedicalWorkflowJson.ToJsonElement(primaryRequest.Metadata),
            primaryRequest.CreatedAt,
            primaryRequest.UpdatedAt);
    }

    private static GroupedTestRequestDto MapGroup(
        IEnumerable<TestRequest> requests,
        IReadOnlyDictionary<string, string> userNames)
    {
        var requestList = requests
            .OrderBy(request => request.Id)
            .ToList();

        var primaryRequest = requestList[0];
        var tests = requestList
            .Select(request => new TestRequestMedicalTestItemDto(
                request.Id,
                request.MedicalTestId,
                request.MedicalTest?.NameEn,
                BuildParameterItems(request.MedicalTest?.ParameterSchema, request.Metadata)))
            .ToList();

        return new(
            primaryRequest.Id,
            tests.Select(test => test.TestRequestId).ToList(),
            tests,
            primaryRequest.DoctorId,
            ResolveUserName(userNames, primaryRequest.DoctorId),
            primaryRequest.LabClientId,
            ResolveUserName(userNames, primaryRequest.LabClientId),
            primaryRequest.DirectPatientId,
            ResolvePatientName(primaryRequest, userNames),
            primaryRequest.ExternalPatientId,
            primaryRequest.ExternalPatient?.FullName,
            primaryRequest.RequestDate,
            primaryRequest.Status,
            primaryRequest.TotalAmount,
            primaryRequest.Notes,
            MedicalWorkflowJson.ToJsonElement(primaryRequest.Metadata),
            primaryRequest.CreatedAt,
            requestList
                .Where(request => request.UpdatedAt.HasValue)
                .Select(request => request.UpdatedAt)
                .Max());
    }

    private static TestRequestGroupKey BuildGroupKey(TestRequest request) =>
        new(
            request.CreatedAt,
            request.CreatedByUserId,
            request.RequestDate,
            request.Status,
            request.TotalAmount,
            request.Notes,
            request.DoctorId,
            request.LabClientId,
            request.DirectPatientId,
            request.ExternalPatientId,
            request.Metadata?.RootElement.GetRawText());

    private sealed record TestRequestGroupKey(
        DateTime CreatedAt,
        string? CreatedByUserId,
        DateTime RequestDate,
        string Status,
        double TotalAmount,
        string? Notes,
        string? DoctorId,
        string? LabClientId,
        string? DirectPatientId,
        int? ExternalPatientId,
        string? MetadataJson);

    private sealed record MedicalTestLookupItem(
        string? NameEn,
        JsonDocument? ParameterSchema);

    private static Expression<Func<TestRequest, bool>>? BuildDefaultExactPredicate(string token) =>
        ParseIntPredicate(token, value => r =>
            r.Id == value ||
            r.MedicalTestId == value ||
            r.ExternalPatientId == value);

    private static IReadOnlyList<TestRequestParameterItemDto> BuildParameterItems(
        JsonDocument? parameterSchema,
        JsonDocument? metadata)
    {
        if (parameterSchema is null)
            return [];

        var schema = parameterSchema.RootElement;
        var values = metadata?.RootElement;
        var items = new List<TestRequestParameterItemDto>();

        if (schema.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var schemaItem in schema.EnumerateArray())
            {
                var value = ResolveParameterValue(schemaItem, values, index);
                var mapped = MapSchemaItem(schemaItem, value, index);
                if (mapped is not null)
                    items.Add(mapped);

                index++;
            }

            return items;
        }

        if (schema.ValueKind == JsonValueKind.Object)
        {
            var index = 0;
            foreach (var property in schema.EnumerateObject())
            {
                var value = ResolvePropertyValue(values, property.Name, index);
                items.Add(new TestRequestParameterItemDto(
                    property.Name,
                    null,
                    property.Name,
                    CloneJsonElement(value)));
                index++;
            }
        }

        return items;
    }

    private static TestRequestParameterItemDto? MapSchemaItem(
        JsonElement schemaItem,
        JsonElement? value,
        int index)
    {
        if (schemaItem.ValueKind != JsonValueKind.Object)
        {
            var fallbackName = schemaItem.ToString();
            return string.IsNullOrWhiteSpace(fallbackName)
                ? null
                : new TestRequestParameterItemDto(fallbackName, null, null, CloneJsonElement(value));
        }

        var parameterName =
            GetStringProperty(schemaItem, "parameterName") ??
            GetStringProperty(schemaItem, "name") ??
            GetStringProperty(schemaItem, "nameEn") ??
            GetStringProperty(schemaItem, "label") ??
            GetStringProperty(schemaItem, "title") ??
            GetStringProperty(schemaItem, "key") ??
            GetStringProperty(schemaItem, "code") ??
            $"Parameter {index + 1}";

        var parameterNameAr =
            GetStringProperty(schemaItem, "parameterNameAr") ??
            GetStringProperty(schemaItem, "nameAr") ??
            GetStringProperty(schemaItem, "labelAr") ??
            GetStringProperty(schemaItem, "titleAr");

        var parameterKey =
            GetStringProperty(schemaItem, "key") ??
            GetStringProperty(schemaItem, "code") ??
            GetStringProperty(schemaItem, "id") ??
            GetStringProperty(schemaItem, "name") ??
            GetStringProperty(schemaItem, "parameterName");

        return new TestRequestParameterItemDto(
            parameterName,
            parameterNameAr,
            parameterKey,
            CloneJsonElement(value));
    }

    private static JsonElement? ResolveParameterValue(JsonElement schemaItem, JsonElement? values, int index)
    {
        if (values is null)
            return null;

        if (values.Value.ValueKind == JsonValueKind.Array)
        {
            var valuesArray = values.Value;
            return index < valuesArray.GetArrayLength() ? valuesArray[index] : null;
        }

        if (values.Value.ValueKind != JsonValueKind.Object)
            return index == 0 ? values : null;

        if (schemaItem.ValueKind != JsonValueKind.Object)
            return null;

        foreach (var candidate in GetCandidateKeys(schemaItem))
        {
            if (values.Value.TryGetProperty(candidate, out var objectValue))
                return objectValue;
        }

        return null;
    }

    private static JsonElement? ResolvePropertyValue(JsonElement? values, string propertyName, int index)
    {
        if (values is null)
            return null;

        if (values.Value.ValueKind == JsonValueKind.Object &&
            values.Value.TryGetProperty(propertyName, out var objectValue))
            return objectValue;

        if (values.Value.ValueKind == JsonValueKind.Array)
        {
            var valuesArray = values.Value;
            return index < valuesArray.GetArrayLength() ? valuesArray[index] : null;
        }

        return null;
    }

    private static IEnumerable<string> GetCandidateKeys(JsonElement schemaItem)
    {
        foreach (var key in new[]
                 {
                     GetStringProperty(schemaItem, "key"),
                     GetStringProperty(schemaItem, "code"),
                     GetStringProperty(schemaItem, "id"),
                     GetStringProperty(schemaItem, "name"),
                     GetStringProperty(schemaItem, "parameterName"),
                     GetStringProperty(schemaItem, "nameEn"),
                     GetStringProperty(schemaItem, "nameAr")
                 })
        {
            if (!string.IsNullOrWhiteSpace(key))
                yield return key!;
        }
    }

    private static string? GetStringProperty(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static JsonElement? CloneJsonElement(JsonElement? element) =>
        element is null
            ? null
            : JsonSerializer.Deserialize<JsonElement>(element.Value.GetRawText());

    private static Expression<Func<TestRequest, bool>>? ParseIntPredicate(
        string token,
        Func<int, Expression<Func<TestRequest, bool>>> predicateFactory) =>
        int.TryParse(token, out var value) ? predicateFactory(value) : null;
}
