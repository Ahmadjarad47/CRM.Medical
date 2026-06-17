using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Availabilities.DTOs;
using CRM.Medical.Application.Features.Availabilities.Services;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class AvailabilityService(
    MedicalDbContext db,
    ICurrentUserAccessor currentUser,
    UserManager<User> userManager) : IAvailabilityService
{
    public async Task<IReadOnlyList<AvailabilityDto>> ListAsync(string? userId, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var targetUserId = ResolveTargetUserId(userId);

        var rows = await db.Availabilities
            .AsNoTracking()
            .Where(x => x.UserId == targetUserId)
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

        EnsureCanAccessAvailability(entity.UserId);
        return Map(entity);
    }

    public async Task<AvailabilityDto> CreateAsync(
        string? userId,
        int dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration,
        bool isActive,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        ValidateAvailabilityValues(dayOfWeek, startTime, endTime, slotDuration);

        var targetUserId = ResolveTargetUserId(userId);
        await EnsureProviderUserAsync(targetUserId);
        await EnsureNoWindowOverlapAsync(targetUserId, dayOfWeek, startTime, endTime, null, cancellationToken);

        var entity = new Availability
        {
            UserId = targetUserId,
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
        string? userId,
        int dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration,
        bool isActive,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        ValidateAvailabilityValues(dayOfWeek, startTime, endTime, slotDuration);

        var entity = await db.Availabilities
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Availability '{id}' was not found.");

        EnsureCanAccessAvailability(entity.UserId);

        var targetUserId = string.IsNullOrWhiteSpace(userId)
            ? entity.UserId
            : ResolveTargetUserId(userId);

        await EnsureProviderUserAsync(targetUserId);
        await EnsureNoWindowOverlapAsync(targetUserId, dayOfWeek, startTime, endTime, id, cancellationToken);

        entity.UserId = targetUserId;
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

        var entity = await db.Availabilities
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Availability '{id}' was not found.");

        EnsureCanAccessAvailability(entity.UserId);

        db.Availabilities.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureProviderUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ApplicationBadRequestException("Target user was not found.");

        var isDoctor = await userManager.IsInRoleAsync(user, UserRoles.Doctor);
        var isLabPartner = await userManager.IsInRoleAsync(user, UserRoles.LabPartner);
        if (!isDoctor && !isLabPartner)
            throw new ApplicationBadRequestException("Availability can only be defined for Doctor or LabPartner accounts.");
    }

    private async Task EnsureNoWindowOverlapAsync(
        string userId,
        int dayOfWeek,
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

    private string ResolveTargetUserId(string? userId)
    {
        var actorId = currentUser.GetRequiredUserId();
        var targetUserId = string.IsNullOrWhiteSpace(userId) ? actorId : userId.Trim();

        if (!IsAdmin() && !string.Equals(targetUserId, actorId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You can only manage your own availability.");

        return targetUserId;
    }

    private void EnsureCanAccessAvailability(string ownerUserId)
    {
        var actorId = currentUser.GetRequiredUserId();
        if (!IsAdmin() && !string.Equals(ownerUserId, actorId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You cannot access this availability.");
    }

    private bool IsAdmin() => currentUser.IsInRole(UserRoles.Admin);

    private static void ValidateAvailabilityValues(
        int dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration)
    {
        if (dayOfWeek is < 0 or > 6)
            throw new ApplicationBadRequestException("DayOfWeek must be in range [0..6].");

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
