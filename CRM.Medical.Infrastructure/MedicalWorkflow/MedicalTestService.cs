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
            ["category"] = t => t.Category,
            ["sample"] = t => t.SampleType,
            ["status"] = t => t.Status
        };

    private static readonly IReadOnlyDictionary<string, Func<string, Expression<Func<MedicalTest, bool>>?>> ExactSearchFields =
        new Dictionary<string, Func<string, Expression<Func<MedicalTest, bool>>?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = token => ParseIntPredicate(token, value => t => t.Id == value)
        };

    public async Task<PagedResult<MedicalTestDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        //MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = await accessPolicyEvaluator.ApplyAsync(db.MedicalTests.AsNoTracking(), "medical_tests", "read", cancellationToken);

        query = query.ApplyAdvancedSearch(
            search,
            SearchFields,
            ExactSearchFields,
            BuildDefaultExactPredicate,
            t => t.NameAr,
            t => t.NameEn,
            t => t.Category,
            t => t.SampleType,
            t => t.Status);

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
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        var entity = await (await accessPolicyEvaluator.ApplyAsync(db.MedicalTests.AsNoTracking(), "medical_tests", "read", cancellationToken))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");

        return Map(entity);
    }

    public async Task<MedicalTestDto> CreateAsync(
        string nameAr,
        string nameEn,
        double price,
        string category,
        string sampleType,
        JsonDocument? parameterSchema,
        string status,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        var userId = user.GetRequiredUserId();
        var entity = new MedicalTest
        {
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            Price = price,
            Category = category.Trim(),
            SampleType = sampleType.Trim(),
            ParameterSchema = parameterSchema,
            Status = status.Trim(),
            CreatedByUserId = userId
        };

        var canCreate = await accessPolicyEvaluator.CanAccessAsync(entity, "medical_tests", "create", cancellationToken);
        if (!canCreate)
            throw new ApplicationForbiddenException("You cannot create this medical test.");

        db.MedicalTests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task UpdateAsync(
        int id,
        string nameAr,
        string nameEn,
        double price,
        string category,
        string sampleType,
        JsonDocument? parameterSchema,
        string status,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);

        var entity = await db.MedicalTests.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");
        var canUpdate = await accessPolicyEvaluator.CanAccessAsync(entity, "medical_tests", "update", cancellationToken);
        if (!canUpdate)
            throw new ApplicationForbiddenException("You cannot update this medical test.");

        entity.NameAr = nameAr.Trim();
        entity.NameEn = nameEn.Trim();
        entity.Price = price;
        entity.Category = category.Trim();
        entity.SampleType = sampleType.Trim();
        entity.ParameterSchema = parameterSchema;
        entity.Status = status.Trim();

        await db.SaveChangesAsync(cancellationToken);
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

    private static MedicalTestDto Map(MedicalTest e) =>
        new(
            e.Id,
            e.NameAr,
            e.NameEn,
            e.Price,
            e.Category,
            e.SampleType,
            MedicalWorkflowJson.ToJsonElement(e.ParameterSchema),
            e.Status,
            e.CreatedAt,
            e.UpdatedAt);

    private static Expression<Func<MedicalTest, bool>>? BuildDefaultExactPredicate(string token) =>
        ParseIntPredicate(token, value => t => t.Id == value);

    private static Expression<Func<MedicalTest, bool>>? ParseIntPredicate(
        string token,
        Func<int, Expression<Func<MedicalTest, bool>>> predicateFactory) =>
        int.TryParse(token, out var value) ? predicateFactory(value) : null;
}
