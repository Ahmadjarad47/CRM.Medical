using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence.Repositories;

public sealed class PermissionRepository(MedicalDbContext db, ICacheService cache) : IPermissionRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private static string CacheKey(Guid id) => $"permission:{id}";

    public async Task<IReadOnlyList<PermissionDto>> ListAsync(CancellationToken cancellationToken)
    {
        var rows = await db.Permissions
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return rows.Select(Map).ToList();
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = CacheKey(id);
        var cached = await cache.GetAsync<PermissionDto>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var row = await db.Permissions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (row is null)
            return null;

        var dto = Map(row);
        await cache.SetAsync(key, dto, CacheDuration, cancellationToken);
        return dto;
    }

    public async Task<PermissionDto?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var row = await db.Permissions.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        return row is null ? null : Map(row);
    }

    public async Task<IReadOnlySet<string>> GetAllNamesAsync(CancellationToken cancellationToken)
    {
        var names = await db.Permissions.AsNoTracking()
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        return names.ToHashSet(StringComparer.Ordinal);
    }

    public async Task<PermissionDto> CreateAsync(
        string name,
        string? description,
        DateTime createdAtUtc,
        CancellationToken cancellationToken)
    {
        var entity = new Permission
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = createdAtUtc
        };

        db.Permissions.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        var dto = Map(entity);
        await cache.SetAsync(CacheKey(entity.Id), dto, CacheDuration, cancellationToken);
        return dto;
    }

    public async Task UpdateDescriptionAsync(Guid id, string? description, CancellationToken cancellationToken)
    {
        var entity = await db.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null)
            return;

        entity.Description = description;
        await db.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(id), cancellationToken);
    }

    public async Task<PermissionDeletionOutcome?> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await db.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null)
            return null;

        var userIds = await db.UserClaims
            .AsNoTracking()
            .Where(c => c.ClaimType == UserPermissions.ClaimType && c.ClaimValue == entity.Name)
            .Select(c => c.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        await db.UserClaims
            .Where(c => c.ClaimType == UserPermissions.ClaimType && c.ClaimValue == entity.Name)
            .ExecuteDeleteAsync(cancellationToken);

        db.Permissions.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(id), cancellationToken);

        return new PermissionDeletionOutcome(userIds);
    }

    private static PermissionDto Map(Permission p) =>
        new(p.Id, p.Name, p.Description, p.CreatedAt);
}
