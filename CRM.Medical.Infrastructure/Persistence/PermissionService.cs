using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class PermissionService(MedicalDbContext db, IDateTimeProvider dateTimeProvider)
    : IPermissionService
{
    public async Task<PermissionDto> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var trimmed = name.Trim();
        if (string.IsNullOrEmpty(trimmed))
            throw new ApplicationBadRequestException("Permission name is required.");

        var exists = await db.Permissions.AnyAsync(p => p.Name == trimmed, cancellationToken);
        if (exists)
            throw new ApplicationConflictException($"A permission named '{trimmed}' already exists.");

        var entity = new Permission
        {
            Id = Guid.NewGuid(),
            Name = trimmed,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CreatedAt = dateTimeProvider.UtcNow
        };

        db.Permissions.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task UpdateAsync(
        Guid id,
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var entity = await db.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Permission '{id}' not found.");

        var trimmed = name.Trim();
        if (string.IsNullOrEmpty(trimmed))
            throw new ApplicationBadRequestException("Permission name is required.");

        var nameTaken = await db.Permissions.AnyAsync(
            p => p.Id != id && p.Name == trimmed,
            cancellationToken);
        if (nameTaken)
            throw new ApplicationConflictException($"A permission named '{trimmed}' already exists.");

        entity.Name = trimmed;
        entity.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await db.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Permission '{id}' not found.");

        db.Permissions.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        return await db.Permissions
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new PermissionDto(p.Id, p.Name, p.Description, p.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    private static PermissionDto ToDto(Permission p) =>
        new(p.Id, p.Name, p.Description, p.CreatedAt);
}
