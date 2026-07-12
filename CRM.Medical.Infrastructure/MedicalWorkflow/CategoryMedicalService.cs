using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using CRM.Medical.Application.Features.CategoryMedical.Services;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class CategoryMedicalService(
    MedicalDbContext db,
    ICurrentUserAccessor user,
    IAccessPolicyEvaluator accessPolicyEvaluator,
    IFileStorageService fileStorage)
    : ICategoryMedicalService
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<CategoryMedical, string?>>> SearchFields =
        new Dictionary<string, Expression<Func<CategoryMedical, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["namear"] = c => c.NameAr,
            ["nameen"] = c => c.NameEn,
            ["description"] = c => c.Description
        };

    private static readonly IReadOnlyDictionary<string, Func<string, Expression<Func<CategoryMedical, bool>>?>> ExactSearchFields =
        new Dictionary<string, Func<string, Expression<Func<CategoryMedical, bool>>?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = token => ParseIntPredicate(token, value => c => c.Id == value)
        };

    public async Task<PagedResult<CategoryMedicalDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        bool? activeOnly,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.CategoryMedical.AsNoTracking(),
            "category_medical",
            "read",
            cancellationToken);

        if (activeOnly == true)
            query = query.Where(c => c.IsActive);

        query = query.ApplyAdvancedSearch(
            search,
            SearchFields,
            ExactSearchFields,
            BuildDefaultExactPredicate,
            c => c.NameAr,
            c => c.NameEn,
            c => c.Description);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Id)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CategoryMedicalDto>
        {
            Items = items.Select(Map).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<CategoryMedicalDto>> ListAllAsync(
        bool activeOnly,
        CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(
            db.CategoryMedical.AsNoTracking(),
            "category_medical",
            "read",
            cancellationToken);

        if (activeOnly)
            query = query.Where(c => c.IsActive);

        var items = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Id)
            .ToListAsync(cancellationToken);

        return items.Select(Map).ToList();
    }

    public async Task<CategoryMedicalDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await (await accessPolicyEvaluator.ApplyAsync(
                db.CategoryMedical.AsNoTracking(),
                "category_medical",
                "read",
                cancellationToken))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical category '{id}' was not found.");

        return Map(entity);
    }

    public async Task<CategoryMedicalDto> CreateAsync(
        string nameAr,
        string nameEn,
        string? description,
        IFormFile? image,
        int displayOrder,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var userId = user.GetRequiredUserId();
        var entity = new CategoryMedical
        {
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            Description = description?.Trim(),
            DisplayOrder = displayOrder,
            IsActive = isActive,
            CreatedByUserId = userId
        };

        var canCreate = await accessPolicyEvaluator.CanAccessAsync(entity, "category_medical", "create", cancellationToken);
        if (!canCreate)
            throw new ApplicationForbiddenException("You cannot create this medical category.");

        if (image is { Length: > 0 })
            entity.ImageUrl = await fileStorage.UploadImageAsync(image, cancellationToken);

        db.CategoryMedical.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task UpdateAsync(
        int id,
        string nameAr,
        string nameEn,
        string? description,
        IFormFile? image,
        int displayOrder,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var entity = await db.CategoryMedical.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical category '{id}' was not found.");

        var canUpdate = await accessPolicyEvaluator.CanAccessAsync(entity, "category_medical", "update", cancellationToken);
        if (!canUpdate)
            throw new ApplicationForbiddenException("You cannot update this medical category.");

        entity.NameAr = nameAr.Trim();
        entity.NameEn = nameEn.Trim();
        entity.Description = description?.Trim();
        if (image is { Length: > 0 })
            entity.ImageUrl = await fileStorage.UploadImageAsync(image, cancellationToken);

        entity.DisplayOrder = displayOrder;
        entity.IsActive = isActive;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.CategoryMedical.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical category '{id}' was not found.");

        var canDelete = await accessPolicyEvaluator.CanAccessAsync(entity, "category_medical", "delete", cancellationToken);
        if (!canDelete)
            throw new ApplicationForbiddenException("You cannot delete this medical category.");

        var inUse = await db.MedicalTests.AnyAsync(t => t.CategoryMedicalId == id, cancellationToken);
        if (inUse)
            throw new ApplicationConflictException("Cannot delete a medical category that has medical tests.");

        db.CategoryMedical.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static CategoryMedicalDto Map(CategoryMedical e) =>
        new(
            e.Id,
            e.NameAr,
            e.NameEn,
            e.Description,
            e.ImageUrl,
            e.DisplayOrder,
            e.IsActive,
            e.CreatedAt,
            e.UpdatedAt);

    private static Expression<Func<CategoryMedical, bool>>? BuildDefaultExactPredicate(string token) =>
        ParseIntPredicate(token, value => c => c.Id == value);

    private static Expression<Func<CategoryMedical, bool>>? ParseIntPredicate(
        string token,
        Func<int, Expression<Func<CategoryMedical, bool>>> predicateFactory) =>
        int.TryParse(token, out var value) ? predicateFactory(value) : null;
}
