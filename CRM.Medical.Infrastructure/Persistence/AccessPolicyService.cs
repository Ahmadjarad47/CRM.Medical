using System.Text.Json;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class AccessPolicyService(
    MedicalDbContext db,
    IConditionParser conditionParser) : IAccessPolicyService
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<AccessPolicy, string?>>> SearchFields =
        new Dictionary<string, Expression<Func<AccessPolicy, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = p => p.Name,
            ["resource"] = p => p.Resource,
            ["action"] = p => p.Action,
            ["subject"] = p => p.SubjectId,
            ["description"] = p => p.Description
        };

    public async Task<AccessPolicyDto> CreateAsync(
        string name,
        string resource,
        string action,
        SubjectType subjectType,
        string subjectId,
        PolicyEffect effect,
        int priority,
        JsonDocument? conditionJson,
        string? description,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var model = ValidateAndNormalize(name, resource, action, subjectType, subjectId, effect, priority, conditionJson, description, isActive);
        await EnsureNoDuplicateActivePolicyAsync(null, model, cancellationToken);

        var entity = new AccessPolicy
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Resource = model.Resource,
            Action = model.Action,
            SubjectType = model.SubjectType,
            SubjectId = model.SubjectId,
            Effect = model.Effect,
            Priority = model.Priority,
            Condition = model.ConditionJson,
            Description = model.Description,
            IsEnabled = isActive
        };

        db.AccessPolicies.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task UpdateAsync(
        Guid id,
        string name,
        string resource,
        string action,
        SubjectType subjectType,
        string subjectId,
        PolicyEffect effect,
        int priority,
        JsonDocument? conditionJson,
        string? description,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var entity = await db.AccessPolicies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Access policy '{id}' not found.");

        var model = ValidateAndNormalize(name, resource, action, subjectType, subjectId, effect, priority, conditionJson, description, isActive);
        await EnsureNoDuplicateActivePolicyAsync(id, model, cancellationToken);

        entity.Name = model.Name;
        entity.Resource = model.Resource;
        entity.Action = model.Action;
        entity.SubjectType = model.SubjectType;
        entity.SubjectId = model.SubjectId;
        entity.Effect = model.Effect;
        entity.Priority = model.Priority;
        entity.Condition = model.ConditionJson;
        entity.Description = model.Description;
        entity.IsEnabled = isActive;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await db.AccessPolicies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Access policy '{id}' not found.");
        db.AccessPolicies.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<AccessPolicyDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = db.AccessPolicies
            .AsNoTracking()
            .ApplyAdvancedSearch(search, SearchFields, p => p.Name, p => p.Resource, p => p.Action, p => p.SubjectId, p => p.Description);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.Name)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .Select(x => new AccessPolicyDto(
                x.Id,
                x.Name,
                x.Resource,
                x.Action,
                x.SubjectType,
                x.SubjectId,
                x.Effect,
                x.Priority,
                x.Condition,
                x.Description,
                x.IsEnabled,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<AccessPolicyDto>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    private async Task EnsureNoDuplicateActivePolicyAsync(
     Guid? id,
     NormalizedPolicyInput model,
     CancellationToken cancellationToken)
    {
        if (!model.IsActive)
            return;

        var candidates = await db.AccessPolicies
            .AsNoTracking()
            .Where(x =>
                (!id.HasValue || x.Id != id.Value) &&
                x.IsEnabled &&
                x.Resource == model.Resource &&
                x.Action == model.Action &&
                x.SubjectType == model.SubjectType &&
                x.SubjectId == model.SubjectId &&
                x.Effect == model.Effect)
            .Select(x => new
            {
                x.Id,
                x.Condition
            })
            .ToListAsync(cancellationToken);

        var newCondition = NormalizeJson(model.ConditionJson);

        var duplicateExists = candidates.Any(x =>
            NormalizeJson(x.Condition) == newCondition);

        if (duplicateExists)
            throw new ApplicationConflictException(
                "An active policy with the same Resource, Action, SubjectType, SubjectId, Effect and Condition already exists.");
    }

    private static string? NormalizeJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        using var doc = JsonDocument.Parse(json);

        return JsonSerializer.Serialize(doc.RootElement);
    }

    private NormalizedPolicyInput ValidateAndNormalize(
        string name,
        string resource,
        string action,
        SubjectType subjectType,
        string subjectId,
        PolicyEffect effect,
        int priority,
        JsonDocument? conditionJson,
        string? description,
        bool isActive)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        var normalizedResource = resource?.Trim() ?? string.Empty;
        var normalizedAction = action?.Trim() ?? string.Empty;
        var normalizedSubjectId = subjectId?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ApplicationBadRequestException("Name is required.");
        if (string.IsNullOrWhiteSpace(normalizedResource))
            throw new ApplicationBadRequestException("Resource is required.");
        if (string.IsNullOrWhiteSpace(normalizedAction))
            throw new ApplicationBadRequestException("Action is required.");
        if (string.IsNullOrWhiteSpace(normalizedSubjectId))
            throw new ApplicationBadRequestException("SubjectId is required.");
        if (priority < 0)
            throw new ApplicationBadRequestException("Priority must be greater than or equal to 0.");
        if (!Enum.IsDefined(subjectType))
            throw new ApplicationBadRequestException("SubjectType is invalid.");
        if (!Enum.IsDefined(effect))
            throw new ApplicationBadRequestException("Effect is invalid.");

        string? normalizedCondition = null;
        if (conditionJson is not null)
        {
            normalizedCondition = JsonSerializer.Serialize(conditionJson.RootElement);
            try
            {
                _ = conditionParser.ParseOrCompile(normalizedCondition);
            }
            catch (Exception ex) when (ex is InvalidOperationException or JsonException)
            {
                throw new ApplicationBadRequestException($"ConditionJson is invalid or unsupported: {ex.Message}");
            }
        }

        return new NormalizedPolicyInput(
            normalizedName,
            normalizedResource,
            normalizedAction,
            subjectType,
            normalizedSubjectId,
            effect,
            priority,
            normalizedCondition,
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            isActive);
    }

    private static AccessPolicyDto ToDto(AccessPolicy x) =>
        new(
            x.Id,
            x.Name,
            x.Resource,
            x.Action,
            x.SubjectType,
            x.SubjectId,
            x.Effect,
            x.Priority,
            x.Condition,
            x.Description,
            x.IsEnabled,
            x.CreatedAt,
            x.UpdatedAt);

    private sealed record NormalizedPolicyInput(
        string Name,
        string Resource,
        string Action,
        SubjectType SubjectType,
        string SubjectId,
        PolicyEffect Effect,
        int Priority,
        string? ConditionJson,
        string? Description,
        bool IsActive);
}
