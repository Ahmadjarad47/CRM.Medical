using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Time;
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
    UserManager<User> userManager,
    IDateTimeProvider dateTimeProvider) : IAppointmentService
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
        int availabilityId,
        int? testRequestId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        string patientLocationType,
        int? age,
        string? gender,
        double? patientLatitude,
        double? patientLongitude,
        string? notes,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        ValidatePatientLocation(patientLocationType, patientLatitude, patientLongitude);
        ValidateTimeRange(startTimeUtc, endTimeUtc);

        var availability = await GetAvailabilityAsync(availabilityId, cancellationToken);
        ValidateRequestedWindowMatchesAvailability(availability, startTimeUtc, endTimeUtc);

        var providerUserId = availability.UserId;
        if (testRequestId.HasValue)
        {
            var testRequest = await db.TestRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == testRequestId.Value, cancellationToken)
                ?? throw new ApplicationNotFoundException($"Test request '{testRequestId.Value}' was not found.");

            EnsureRequestCanBeBooked(testRequest);
        }

        await EnsureNoAppointmentOverlapAsync(providerUserId, startTimeUtc, endTimeUtc, null, cancellationToken);

        var entity = new Appointment
        {
            AvailabilityId = availability.Id,
            TestRequestId = testRequestId,
            ProviderUserId = providerUserId,
            StartTime = startTimeUtc,
            EndTime = endTimeUtc,
            Status = AppointmentStatuses.Scheduled,
            PatientLocationType = NormalizePatientLocationType(patientLocationType),
            Age = age,
            Gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim(),
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
        int availabilityId,
        int testRequestId,
        string? userId,
        string patientLocationType,
        int? age,
        string? gender,
        double? patientLatitude,
        double? patientLongitude,
        string? notes,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        EnsureAllowedSchedulingRole();

        ValidatePatientLocation(patientLocationType, patientLatitude, patientLongitude);

        var entity = await db.Appointments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{id}' was not found.");

        EnsureCanAccessAppointment(entity.ProviderUserId);

        if (string.Equals(entity.Status, AppointmentStatuses.Cancelled, StringComparison.Ordinal))
            throw new ApplicationBadRequestException("Cancelled appointments cannot be updated.");

        var availability = await GetAvailabilityAsync(availabilityId, cancellationToken);
        var referenceUtc = entity.StartTime == default ? dateTimeProvider.UtcNow : entity.StartTime;
        var (startTimeUtc, endTimeUtc) = ResolveAppointmentWindowFromAvailability(availability, referenceUtc);
        ValidateTimeRange(startTimeUtc, endTimeUtc);

        var providerUserId = ResolveProviderUserIdFromAvailability(userId, availability);
        var (isDoctor, isLabPartner) = await EnsureProviderRoleAsync(providerUserId);

        var testRequest = await db.TestRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{testRequestId}' was not found.");

        EnsureProviderMatchesTestRequest(testRequest, providerUserId, isDoctor, isLabPartner);
        EnsureRequestCanBeBooked(testRequest);

        await EnsureNoAppointmentOverlapAsync(providerUserId, startTimeUtc, endTimeUtc, id, cancellationToken);

        entity.AvailabilityId = availability.Id;
        entity.TestRequestId = testRequestId;
        entity.ProviderUserId = providerUserId;
        entity.StartTime = startTimeUtc;
        entity.EndTime = endTimeUtc;
        entity.PatientLocationType = NormalizePatientLocationType(patientLocationType);
        entity.Age = age;
        entity.Gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim();
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


        var dayStart = NormalizeToUtcDayStart(date);
        var dayEnd = dayStart.AddDays(1);
        var dayOfWeek = dayStart.DayOfWeek;

        var windows = await db.Availabilities
            .AsNoTracking()
            .Where(x =>
               
                x.DayOfWeek == dayOfWeek &&
                x.IsActive)
            .OrderBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

        var appointments = await db.Appointments
            .AsNoTracking()
            .Where(x =>
              
                x.Status != AppointmentStatuses.Cancelled &&
                x.StartTime < dayEnd &&
                x.EndTime > dayStart)
            .OrderBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

        var windowDtos = windows
            .Select(window => new AppointmentAvailabilityWindowDto(
                window.Id,
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
                    window.Id,
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

    private static DateTime NormalizeToUtcDayStart(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value.Date,
            DateTimeKind.Local => value.ToUniversalTime().Date,
            _ => DateTime.SpecifyKind(value.Date, DateTimeKind.Utc)
        };

    private async Task<Availability> GetAvailabilityAsync(
        int availabilityId,
        CancellationToken cancellationToken)
    {
        var availability = await db.Availabilities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == availabilityId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Availability '{availabilityId}' was not found.");

        if (!availability.IsActive)
            throw new ApplicationBadRequestException("The selected availability is inactive.");

        return availability;
    }

    private static (DateTime StartTimeUtc, DateTime EndTimeUtc) ResolveAppointmentWindowFromAvailability(
        Availability availability,
        DateTime referenceUtc)
    {
        var referenceDate = referenceUtc.Date;
        var daysUntil = ((int)availability.DayOfWeek - (int)referenceDate.DayOfWeek + 7) % 7;
        var targetDate = referenceDate.AddDays(daysUntil);

        var startTimeUtc = targetDate.Add(availability.StartTime);
        var endTimeUtc = targetDate.Add(availability.EndTime);

        if (daysUntil == 0 && startTimeUtc < referenceUtc)
        {
            targetDate = targetDate.AddDays(7);
            startTimeUtc = targetDate.Add(availability.StartTime);
            endTimeUtc = targetDate.Add(availability.EndTime);
        }

        return (startTimeUtc, endTimeUtc);
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

    private static void ValidateRequestedWindowMatchesAvailability(
        Availability availability,
        DateTime startTimeUtc,
        DateTime endTimeUtc)
    {
        if (startTimeUtc.DayOfWeek != availability.DayOfWeek)
            throw new ApplicationBadRequestException("StartTime must match the selected availability day.");

        var requestedStart = startTimeUtc.TimeOfDay;
        var requestedEnd = endTimeUtc.TimeOfDay;
        if (requestedStart < availability.StartTime || requestedEnd > availability.EndTime)
            throw new ApplicationBadRequestException("StartTime/EndTime must be within the selected availability window.");
    }

    private async Task<(bool IsDoctor, bool IsLabPartner)> EnsureProviderRoleAsync(string providerUserId)
    {
        var provider = await userManager.FindByIdAsync(providerUserId)
            ?? throw new ApplicationBadRequestException("Assigned provider was not found.");

        var isDoctor = await userManager.IsInRoleAsync(provider, UserRoles.Doctor);
        var isLabPartner = await userManager.IsInRoleAsync(provider, UserRoles.LabPartner);
        //if (!isDoctor && !isLabPartner)
        //    throw new ApplicationBadRequestException("Appointment provider must be Doctor or LabPartner.");

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

    private string ResolveProviderUserIdFromAvailability(string? userId, Availability availability)
    {
        var actorId = currentUser.GetRequiredUserId();
        var providerUserId = availability.UserId;

        if (!string.IsNullOrWhiteSpace(userId) &&
            !string.Equals(userId.Trim(), providerUserId, StringComparison.Ordinal))
            throw new ApplicationBadRequestException("UserId must match the owner of the selected availability.");

        if (!IsAdmin() && !string.Equals(providerUserId, actorId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You can only create or update appointments for your own account.");

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
        if (IsAdmin() || currentUser.IsInRole(UserRoles.Doctor) || currentUser.IsInRole(UserRoles.LabPartner)|| currentUser.IsInRole(UserRoles.Patient))
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
            entity.AvailabilityId,
            entity.TestRequestId,
            entity.ProviderUserId,
            entity.StartTime,
            entity.EndTime,
            entity.Status,
            entity.PatientLocationType,
            entity.Age,
            entity.Gender,
            entity.PatientLatitude,
            entity.PatientLongitude,
            entity.Notes,
            entity.CreatedByUserId,
            entity.CreatedAt,
            entity.UpdatedAt);
}
