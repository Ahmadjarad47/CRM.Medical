using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Appointments.Services;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class AppointmentService(
    MedicalDbContext db,
    ICurrentUserAccessor currentUser,
    UserManager<User> userManager) : IAppointmentService
{
    public async Task<IReadOnlyList<AppointmentDto>> ListAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        string? userId,
        string? status,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAllowedSchedulingRole();

        if (fromUtc.HasValue && toUtc.HasValue && fromUtc.Value > toUtc.Value)
            throw new ApplicationBadRequestException("FromUtc must be less than or equal to ToUtc.");

        var actorId = currentUser.GetRequiredUserId();
        var query = db.Appointments.AsNoTracking();

        if (IsAdmin())
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var targetUserId = userId.Trim();
                query = query.Where(x => x.ProviderUserId == targetUserId);
            }
        }
        else
        {
            query = query.Where(x => x.ProviderUserId == actorId);
            if (!string.IsNullOrWhiteSpace(userId) &&
                !string.Equals(userId.Trim(), actorId, StringComparison.Ordinal))
                throw new ApplicationForbiddenException("You can only list your own appointments.");
        }

        if (fromUtc.HasValue)
            query = query.Where(x => x.StartTime >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(x => x.EndTime <= toUtc.Value);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim();
            query = query.Where(x => x.Status == normalizedStatus);
        }

        var rows = await query
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(cancellationToken);

        return rows.Select(Map).ToList();
    }

    public async Task<AppointmentDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAllowedSchedulingRole();

        var entity = await db.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{id}' was not found.");

        EnsureCanAccessAppointment(entity.ProviderUserId);
        return Map(entity);
    }

    public async Task<AppointmentDto> CreateAsync(
        int testRequestId,
        string? userId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        string patientLocationType,
        double? patientLatitude,
        double? patientLongitude,
        string? notes,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAllowedSchedulingRole();

        ValidateTimeRange(startTimeUtc, endTimeUtc);
        ValidatePatientLocation(patientLocationType, patientLatitude, patientLongitude);

        var providerUserId = ResolveProviderUserId(userId);
        var (isDoctor, isLabPartner) = await EnsureProviderRoleAsync(providerUserId);

        var testRequest = await db.TestRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{testRequestId}' was not found.");

        EnsureProviderMatchesTestRequest(testRequest, providerUserId, isDoctor, isLabPartner);
        EnsureRequestCanBeBooked(testRequest);

        await EnsureAvailabilityWindowAsync(providerUserId, startTimeUtc, endTimeUtc, cancellationToken);
        await EnsureNoAppointmentOverlapAsync(providerUserId, startTimeUtc, endTimeUtc, null, cancellationToken);

        var entity = new Appointment
        {
            TestRequestId = testRequestId,
            ProviderUserId = providerUserId,
            StartTime = startTimeUtc,
            EndTime = endTimeUtc,
            Status = AppointmentStatuses.Scheduled,
            PatientLocationType = NormalizePatientLocationType(patientLocationType),
            PatientLatitude = patientLatitude,
            PatientLongitude = patientLongitude,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            CreatedByUserId = currentUser.GetRequiredUserId()
        };

        db.Appointments.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task UpdateAsync(
        int id,
        int testRequestId,
        string? userId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        string patientLocationType,
        double? patientLatitude,
        double? patientLongitude,
        string? notes,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAllowedSchedulingRole();

        ValidateTimeRange(startTimeUtc, endTimeUtc);
        ValidatePatientLocation(patientLocationType, patientLatitude, patientLongitude);

        var entity = await db.Appointments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{id}' was not found.");

        EnsureCanAccessAppointment(entity.ProviderUserId);

        if (string.Equals(entity.Status, AppointmentStatuses.Cancelled, StringComparison.Ordinal))
            throw new ApplicationBadRequestException("Cancelled appointments cannot be updated.");

        var providerUserId = ResolveProviderUserIdForUpdate(userId, entity.ProviderUserId);
        var (isDoctor, isLabPartner) = await EnsureProviderRoleAsync(providerUserId);

        var testRequest = await db.TestRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{testRequestId}' was not found.");

        EnsureProviderMatchesTestRequest(testRequest, providerUserId, isDoctor, isLabPartner);
        EnsureRequestCanBeBooked(testRequest);

        await EnsureAvailabilityWindowAsync(providerUserId, startTimeUtc, endTimeUtc, cancellationToken);
        await EnsureNoAppointmentOverlapAsync(providerUserId, startTimeUtc, endTimeUtc, id, cancellationToken);

        entity.TestRequestId = testRequestId;
        entity.ProviderUserId = providerUserId;
        entity.StartTime = startTimeUtc;
        entity.EndTime = endTimeUtc;
        entity.PatientLocationType = NormalizePatientLocationType(patientLocationType);
        entity.PatientLatitude = patientLatitude;
        entity.PatientLongitude = patientLongitude;
        entity.Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAllowedSchedulingRole();

        var entity = await db.Appointments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{id}' was not found.");

        EnsureCanAccessAppointment(entity.ProviderUserId);

        if (string.Equals(entity.Status, AppointmentStatuses.Cancelled, StringComparison.Ordinal))
            return;

        entity.Status = AppointmentStatuses.Cancelled;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<AppointmentDayAvailabilityDto> GetDayAvailabilityAsync(
        DateTime date,
        string? userId,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAllowedSchedulingRole();

        if (date == default)
            throw new ApplicationBadRequestException("Date is required.");

        var providerUserId = ResolveProviderUserIdForRead(userId);
        await EnsureProviderRoleAsync(providerUserId);

        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);
        var dayOfWeek = (int)dayStart.DayOfWeek;

        var windows = await db.Availabilities
            .AsNoTracking()
            .Where(x =>
                x.UserId == providerUserId &&
                x.DayOfWeek == dayOfWeek &&
                x.IsActive)
            .OrderBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

        var appointments = await db.Appointments
            .AsNoTracking()
            .Where(x =>
                x.ProviderUserId == providerUserId &&
                x.Status != AppointmentStatuses.Cancelled &&
                x.StartTime < dayEnd &&
                x.EndTime > dayStart)
            .OrderBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

        var windowDtos = windows
            .Select(window => new AppointmentAvailabilityWindowDto(
                dayStart.Add(window.StartTime),
                dayStart.Add(window.EndTime),
                window.SlotDuration))
            .ToList();

        var slots = new List<AppointmentAvailabilitySlotDto>();

        foreach (var window in windows)
        {
            var slotDuration = TimeSpan.FromMinutes(window.SlotDuration);
            var windowStart = dayStart.Add(window.StartTime);
            var windowEnd = dayStart.Add(window.EndTime);

            for (var slotStart = windowStart; slotStart + slotDuration <= windowEnd; slotStart += slotDuration)
            {
                var slotEnd = slotStart + slotDuration;
                var overlap = appointments.FirstOrDefault(x =>
                    slotStart < x.EndTime &&
                    slotEnd > x.StartTime);

                slots.Add(new AppointmentAvailabilitySlotDto(
                    slotStart,
                    slotEnd,
                    window.SlotDuration,
                    overlap is null,
                    overlap?.Id,
                    overlap?.TestRequestId));
            }
        }

        var totalSlots = slots.Count;
        var availableSlots = slots.Count(x => x.IsAvailable);

        return new AppointmentDayAvailabilityDto(
            providerUserId,
            dayStart,
            windowDtos,
            slots,
            totalSlots,
            availableSlots,
            totalSlots - availableSlots);
    }

    private async Task EnsureAvailabilityWindowAsync(
        string providerUserId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        CancellationToken cancellationToken)
    {
        var dayOfWeek = (int)startTimeUtc.DayOfWeek;
        var startOfDay = startTimeUtc.TimeOfDay;
        var endOfDay = endTimeUtc.TimeOfDay;
        var duration = endTimeUtc - startTimeUtc;

        var windows = await db.Availabilities
            .AsNoTracking()
            .Where(x =>
                x.UserId == providerUserId &&
                x.DayOfWeek == dayOfWeek &&
                x.IsActive)
            .OrderBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

        if (windows.Count == 0)
            throw new ApplicationBadRequestException("No active availability was found for the provider on the requested day.");

        foreach (var window in windows)
        {
            if (startOfDay < window.StartTime || endOfDay > window.EndTime)
                continue;

            var slot = TimeSpan.FromMinutes(window.SlotDuration);
            var offset = startOfDay - window.StartTime;
            if (offset.Ticks % slot.Ticks != 0)
                continue;

            if (duration.Ticks % slot.Ticks != 0)
                continue;

            return;
        }

        throw new ApplicationBadRequestException("Requested appointment time is outside configured provider availability or slot boundaries.");
    }

    private async Task EnsureNoAppointmentOverlapAsync(
        string providerUserId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        int? excludeAppointmentId,
        CancellationToken cancellationToken)
    {
        var hasOverlap = await db.Appointments
            .AsNoTracking()
            .AnyAsync(
                x => x.ProviderUserId == providerUserId &&
                     x.Id != (excludeAppointmentId ?? 0) &&
                     x.Status != AppointmentStatuses.Cancelled &&
                     startTimeUtc < x.EndTime &&
                     endTimeUtc > x.StartTime,
                cancellationToken);

        if (hasOverlap)
            throw new ApplicationConflictException("Provider already has an appointment in this time range.");
    }

    private async Task<(bool IsDoctor, bool IsLabPartner)> EnsureProviderRoleAsync(string providerUserId)
    {
        var provider = await userManager.FindByIdAsync(providerUserId)
            ?? throw new ApplicationBadRequestException("Assigned provider was not found.");

        var isDoctor = await userManager.IsInRoleAsync(provider, UserRoles.Doctor);
        var isLabPartner = await userManager.IsInRoleAsync(provider, UserRoles.LabPartner);
        if (!isDoctor && !isLabPartner)
            throw new ApplicationBadRequestException("Appointment provider must be Doctor or LabPartner.");

        return (isDoctor, isLabPartner);
    }

    private static void EnsureProviderMatchesTestRequest(
        TestRequest testRequest,
        string providerUserId,
        bool isDoctor,
        bool isLabPartner)
    {
        var matchesDoctor = isDoctor && string.Equals(testRequest.DoctorId, providerUserId, StringComparison.Ordinal);
        var matchesLabPartner = isLabPartner && string.Equals(testRequest.LabClientId, providerUserId, StringComparison.Ordinal);

        if (!matchesDoctor && !matchesLabPartner)
            throw new ApplicationBadRequestException("The selected provider is not assigned to this test request.");
    }

    private static void EnsureRequestCanBeBooked(TestRequest testRequest)
    {
        if (string.Equals(testRequest.Status, TestRequestStatuses.Cancelled, StringComparison.OrdinalIgnoreCase))
            throw new ApplicationBadRequestException("Cannot create appointments for cancelled test requests.");

        if (string.Equals(testRequest.Status, TestRequestStatuses.Completed, StringComparison.OrdinalIgnoreCase))
            throw new ApplicationBadRequestException("Cannot create appointments for completed test requests.");
    }

    private static void ValidateTimeRange(DateTime startTimeUtc, DateTime endTimeUtc)
    {
        if (startTimeUtc >= endTimeUtc)
            throw new ApplicationBadRequestException("StartTime must be before EndTime.");

        if (startTimeUtc.Date != endTimeUtc.Date)
            throw new ApplicationBadRequestException("Appointment must start and end on the same day.");
    }

    private string ResolveProviderUserId(string? userId)
    {
        var actorId = currentUser.GetRequiredUserId();
        var providerUserId = string.IsNullOrWhiteSpace(userId) ? actorId : userId.Trim();

        if (!IsAdmin() && !string.Equals(providerUserId, actorId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You can only create appointments for your own account.");

        return providerUserId;
    }

    private string ResolveProviderUserIdForUpdate(string? userId, string currentProviderUserId)
    {
        var actorId = currentUser.GetRequiredUserId();
        var providerUserId = string.IsNullOrWhiteSpace(userId) ? currentProviderUserId : userId.Trim();

        if (!IsAdmin() && !string.Equals(providerUserId, actorId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You can only update appointments for your own account.");

        return providerUserId;
    }

    private string ResolveProviderUserIdForRead(string? userId)
    {
        var actorId = currentUser.GetRequiredUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return actorId;

        var providerUserId = userId.Trim();
        if (!IsAdmin() && !string.Equals(providerUserId, actorId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You can only read your own availability.");

        return providerUserId;
    }

    private void EnsureCanAccessAppointment(string providerUserId)
    {
        var actorId = currentUser.GetRequiredUserId();
        if (!IsAdmin() && !string.Equals(providerUserId, actorId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You cannot access this appointment.");
    }

    private void EnsureAllowedSchedulingRole()
    {
        if (IsAdmin() || currentUser.IsInRole(UserRoles.Doctor) || currentUser.IsInRole(UserRoles.LabPartner))
            return;

        throw new ApplicationForbiddenException("Only Admin, Doctor, or LabPartner can manage appointments.");
    }

    private bool IsAdmin() => currentUser.IsInRole(UserRoles.Admin);

    private static void ValidatePatientLocation(
        string patientLocationType,
        double? patientLatitude,
        double? patientLongitude)
    {
        var normalizedType = NormalizePatientLocationType(patientLocationType);

        if (patientLatitude is < -90 or > 90)
            throw new ApplicationBadRequestException("PatientLatitude must be in range [-90, 90].");

        if (patientLongitude is < -180 or > 180)
            throw new ApplicationBadRequestException("PatientLongitude must be in range [-180, 180].");

        if (normalizedType is AppointmentPatientLocationTypes.Home or AppointmentPatientLocationTypes.Work)
        {
            if (!patientLatitude.HasValue || !patientLongitude.HasValue)
                throw new ApplicationBadRequestException("PatientLatitude and PatientLongitude are required for Home and Work appointments.");
        }
    }

    private static string NormalizePatientLocationType(string locationType)
    {
        if (string.IsNullOrWhiteSpace(locationType))
            throw new ApplicationBadRequestException("PatientLocationType is required.");

        var normalized = locationType.Trim();
        if (AppointmentPatientLocationTypes.All.Any(x => string.Equals(x, normalized, StringComparison.OrdinalIgnoreCase)))
            return AppointmentPatientLocationTypes.All.First(x => string.Equals(x, normalized, StringComparison.OrdinalIgnoreCase));

        throw new ApplicationBadRequestException("PatientLocationType must be Home, Work, or ComeToUs.");
    }

    private static AppointmentDto Map(Appointment entity) =>
        new(
            entity.Id,
            entity.TestRequestId,
            entity.ProviderUserId,
            entity.StartTime,
            entity.EndTime,
            entity.Status,
            entity.PatientLocationType,
            entity.PatientLatitude,
            entity.PatientLongitude,
            entity.Notes,
            entity.CreatedByUserId,
            entity.CreatedAt,
            entity.UpdatedAt);
}
