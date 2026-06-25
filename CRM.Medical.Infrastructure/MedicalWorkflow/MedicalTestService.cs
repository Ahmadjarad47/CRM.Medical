using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Application.Features.MedicalTests.Services;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class MedicalTestService(MedicalDbContext db, ICurrentUserAccessor user, IAccessPolicyEvaluator accessPolicyEvaluator)
    : IMedicalTestService
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<MedicalTest, string?>>> SearchFields =
        new Dictionary<string, Expression<Func<MedicalTest, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["namear"] = t => t.NameAr,
            ["nameen"] = t => t.NameEn,
            ["categorynamear"] = t => t.CategoryMedical.NameAr,
            ["categorynameen"] = t => t.CategoryMedical.NameEn,
            ["sample"] = t => t.SampleType
        };

    private static readonly IReadOnlyDictionary<string, Func<string, Expression<Func<MedicalTest, bool>>?>> ExactSearchFields =
        new Dictionary<string, Func<string, Expression<Func<MedicalTest, bool>>?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = token => ParseIntPredicate(token, value => t => t.Id == value),
            ["categorymedicalid"] = token => ParseIntPredicate(token, value => t => t.CategoryMedicalId == value),
            ["status"] = ParseStatusPredicate
        };

    public async Task<PagedResult<MedicalTestDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        int? categoryMedicalId,
        CancellationToken cancellationToken)
    {
        //MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.MedicalTests.AsNoTracking().Include(t => t.CategoryMedical),
            "medical_tests",
            "read",
            cancellationToken);

        if (categoryMedicalId is not null)
            query = query.Where(t => t.CategoryMedicalId == categoryMedicalId.Value);

        query = query.ApplyAdvancedSearch(
            search,
            SearchFields,
            ExactSearchFields,
            BuildDefaultExactPredicate,
            t => t.NameAr,
            t => t.NameEn,
            t => t.CategoryMedical.NameAr,
            t => t.CategoryMedical.NameEn,
            t => t.SampleType);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(t => t.Id)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<MedicalTestDto>
        {
            Items = items.Select(Map).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    public async Task<MedicalTestDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        //MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        var entity = await (await accessPolicyEvaluator.ApplyAsync(
                db.MedicalTests.AsNoTracking().Include(t => t.CategoryMedical),
                "medical_tests",
                "read",
                cancellationToken))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");

        return Map(entity);
    }

    public async Task<MedicalTestDto> CreateAsync(
        string nameAr,
        string nameEn,
        double price,
        int categoryMedicalId,
        string sampleType,
        JsonDocument? parameterSchema,
        MedicalTestStatus status,
        CancellationToken cancellationToken)
    {
        //MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        await EnsureCategoryMedicalExistsAsync(categoryMedicalId, cancellationToken);

        var userId = user.GetRequiredUserId();
        var entity = new MedicalTest
        {
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            Price = price,
            CategoryMedicalId = categoryMedicalId,
            SampleType = sampleType.Trim(),
            ParameterSchema = parameterSchema,
            Status = status,
            CreatedByUserId = userId
        };

        var canCreate = await accessPolicyEvaluator.CanAccessAsync(entity, "medical_tests", "create", cancellationToken);
        if (!canCreate)
            throw new ApplicationForbiddenException("You cannot create this medical test.");

        db.MedicalTests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(entity).Reference(t => t.CategoryMedical).LoadAsync(cancellationToken);

        return Map(entity);
    }

    public async Task UpdateAsync(
        int id,
        string nameAr,
        string nameEn,
        double price,
        int categoryMedicalId,
        string sampleType,
        JsonDocument? parameterSchema,
        MedicalTestStatus status,
        CancellationToken cancellationToken)
    {
        //MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        await EnsureCategoryMedicalExistsAsync(categoryMedicalId, cancellationToken);

        var entity = await db.MedicalTests
            .Include(t => t.CategoryMedical)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");
        var canUpdate = await accessPolicyEvaluator.CanAccessAsync(entity, "medical_tests", "update", cancellationToken);
        if (!canUpdate)
            throw new ApplicationForbiddenException("You cannot update this medical test.");

        entity.NameAr = nameAr.Trim();
        entity.NameEn = nameEn.Trim();
        entity.Price = price;
        entity.CategoryMedicalId = categoryMedicalId;
        entity.SampleType = sampleType.Trim();
        entity.ParameterSchema = parameterSchema;
        entity.Status = status;

        await db.SaveChangesAsync(cancellationToken);
        await db.Entry(entity).Reference(t => t.CategoryMedical).LoadAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        var entity = await db.MedicalTests.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");
        var canDelete = await accessPolicyEvaluator.CanAccessAsync(entity, "medical_tests", "delete", cancellationToken);
        if (!canDelete)
            throw new ApplicationForbiddenException("You cannot delete this medical test.");

        var inUse = await db.TestRequests.AnyAsync(r => r.MedicalTestId == id, cancellationToken);
        if (inUse)
            throw new ApplicationConflictException("Cannot delete a medical test that has test requests.");

        db.MedicalTests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ToggleStatusAsync(int id, MedicalTestStatus status, CancellationToken cancellationToken)
    {
        var entity = await db.MedicalTests.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");
        var canUpdate = await accessPolicyEvaluator.CanAccessAsync(entity, "medical_tests", "update", cancellationToken);
        if (!canUpdate)
            throw new ApplicationForbiddenException("You cannot update this medical test status.");

        entity.Status = status;
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCategoryMedicalExistsAsync(int categoryMedicalId, CancellationToken cancellationToken)
    {
        var exists = await db.CategoryMedical.AnyAsync(c => c.Id == categoryMedicalId, cancellationToken);
        if (!exists)
            throw new ApplicationNotFoundException($"Medical category '{categoryMedicalId}' was not found.");
    }

    private static MedicalTestDto Map(MedicalTest e) =>
        new(
            e.Id,
            e.NameAr,
            e.NameEn,
            e.Price,
            e.CategoryMedicalId,
            e.CategoryMedical.NameAr,
            e.CategoryMedical.NameEn,
            e.SampleType,
            MedicalWorkflowJson.ToJsonElement(e.ParameterSchema),
            e.Status,
            e.CreatedAt,
            e.UpdatedAt);

    private static Expression<Func<MedicalTest, bool>>? BuildDefaultExactPredicate(string token) =>
        ParseIntPredicate(token, value => t => t.Id == value);

    private static Expression<Func<MedicalTest, bool>>? ParseStatusPredicate(string token) =>
        Enum.TryParse<MedicalTestStatus>(token, true, out var status)
            ? t => t.Status == status
            : null;

    private static Expression<Func<MedicalTest, bool>>? ParseIntPredicate(
        string token,
        Func<int, Expression<Func<MedicalTest, bool>>> predicateFactory) =>
        int.TryParse(token, out var value) ? predicateFactory(value) : null;
}
