using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Availabilities.DTOs;
using CRM.Medical.Application.Features.Availabilities.Services;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class AvailabilityService(
    MedicalDbContext db,
    ICurrentUserAccessor currentUser) : IAvailabilityService
{
    public async Task<IReadOnlyList<AvailabilityDto>> ListAsync(string? userId, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var query = db.Availabilities
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            var targetUserId = userId.Trim();
            query = query.Where(x => x.UserId == targetUserId);
        }

        var rows = await query
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

        return rows.Select(Map).ToList();
    }

    public async Task<AvailabilityDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var entity = await db.Availabilities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Availability '{id}' was not found.");

        return Map(entity);
    }

    public async Task<AvailabilityDto> CreateAsync(
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration,
        bool isActive,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAdminCanMutate();

        ValidateAvailabilityValues(dayOfWeek, startTime, endTime, slotDuration);

        var actorUserId = currentUser.GetRequiredUserId();
        await EnsureNoWindowOverlapAsync(actorUserId, dayOfWeek, startTime, endTime, null, cancellationToken);

        var entity = new Availability
        {
            UserId = actorUserId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            SlotDuration = slotDuration,
            IsActive = isActive,
            CreatedByUserId = currentUser.GetRequiredUserId()
        };

        db.Availabilities.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task UpdateAsync(
        int id,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration,
        bool isActive,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAdminCanMutate();

        ValidateAvailabilityValues(dayOfWeek, startTime, endTime, slotDuration);

        var entity = await db.Availabilities
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Availability '{id}' was not found.");

        await EnsureNoWindowOverlapAsync(entity.UserId, dayOfWeek, startTime, endTime, id, cancellationToken);
        entity.DayOfWeek = dayOfWeek;
        entity.StartTime = startTime;
        entity.EndTime = endTime;
        entity.SlotDuration = slotDuration;
        entity.IsActive = isActive;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAdminCanMutate();

        var entity = await db.Availabilities
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Availability '{id}' was not found.");

        db.Availabilities.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureNoWindowOverlapAsync(
        string userId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var hasOverlap = await db.Availabilities
            .AsNoTracking()
            .AnyAsync(
                x => x.UserId == userId &&
                     x.DayOfWeek == dayOfWeek &&
                     x.Id != (excludeId ?? 0) &&
                     startTime < x.EndTime &&
                     endTime > x.StartTime,
                cancellationToken);

        if (hasOverlap)
            throw new ApplicationConflictException("The availability window overlaps an existing record for this user/day.");
    }

    private bool IsAdmin() => currentUser.IsInRole(UserRoles.Admin);

    private void EnsureAdminCanMutate()
    {
        if (!IsAdmin())
            throw new ApplicationForbiddenException("Only Admin can create, update, or delete availability.");
    }

    private static void ValidateAvailabilityValues(
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration)
    {
        if (!Enum.IsDefined(dayOfWeek))
            throw new ApplicationBadRequestException("DayOfWeek is invalid.");

        if (startTime >= endTime)
            throw new ApplicationBadRequestException("StartTime must be before EndTime.");

        if (slotDuration <= 0)
            throw new ApplicationBadRequestException("SlotDuration must be greater than zero.");

        var duration = endTime - startTime;
        var slotTicks = TimeSpan.FromMinutes(slotDuration).Ticks;
        if (duration.Ticks % slotTicks != 0)
            throw new ApplicationBadRequestException("Availability window length must be a multiple of SlotDuration.");
    }

    private static AvailabilityDto Map(Availability entity) =>
        new(
            entity.Id,
            entity.UserId,
            entity.DayOfWeek,
            entity.StartTime,
            entity.EndTime,
            entity.SlotDuration,
            entity.IsActive,
            entity.CreatedAt,
            entity.UpdatedAt);
}
