using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CRM.Medical.Infrastructure.Persistence;

namespace CRM.Medical.Infrastructure.Chat;

/// <summary>
/// Enforces healthcare chat routing rules using Identity roles and <see cref="TestRequest"/> workflow links.
/// </summary>
public sealed class ChatAuthorizationService(
    IChatPersistence chatPersistence,
    UserManager<User> userManager,
    MedicalDbContext db)
    : IChatAuthorizationService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly MedicalDbContext _db = db;

    public async Task EnsureActiveParticipantAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default)
    {
        var ok = await chatPersistence.IsActiveParticipantAsync(userId, conversationId, cancellationToken);

        if (!ok)
            throw new ApplicationForbiddenException("You are not an active participant in this conversation.");
    }

    public async Task EnsureCanChatWithPeersAsync(
        string actorUserId,
        IReadOnlyCollection<string> otherUserIds,
        CancellationToken cancellationToken = default)
    {
        var allowedPeerIds = await GetPeerUserIdsActorMayChatAsync(actorUserId, cancellationToken);
        var allowedSet = allowedPeerIds.ToHashSet(StringComparer.Ordinal);

        var distinctPeers = otherUserIds
            .Where(id => !string.Equals(id, actorUserId, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        foreach (var peerId in distinctPeers)
        {
            await RequireActivePeerAsync(peerId, cancellationToken);

            if (!allowedSet.Contains(peerId))
                throw new ApplicationForbiddenException(
                    "You are not allowed to start or join a conversation with one or more selected users.");
        }
    }

    public async Task<IReadOnlyList<string>> GetPeerUserIdsActorMayChatAsync(
        string actorUserId,
        CancellationToken cancellationToken = default)
    {
        var actor = await RequireActiveUserAsync(actorUserId, cancellationToken);
        var roles = await _userManager.GetRolesAsync(actor);

        if (roles.Any(role => string.Equals(role, UserRoles.Admin, StringComparison.OrdinalIgnoreCase)))
        {
            return await _db.Users
                .AsNoTracking()
                .Where(user => user.IsActive && user.Id != actorUserId)
                .OrderBy(user => user.FullName)
                .Select(user => user.Id)
                .ToListAsync(cancellationToken);
        }

        var candidateIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (var adminId in await GetActiveUserIdsInRoleAsync(UserRoles.Admin, cancellationToken))
            candidateIds.Add(adminId);

        var isDoctor = roles.Any(role => string.Equals(role, UserRoles.Doctor, StringComparison.OrdinalIgnoreCase));
        var isLabPartner = roles.Any(role => string.Equals(role, UserRoles.LabPartner, StringComparison.OrdinalIgnoreCase));
        var isPatientLike = roles.Any(role =>
            string.Equals(role, UserRoles.Patient, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, UserRoles.User, StringComparison.OrdinalIgnoreCase));

        if (isPatientLike)
        {
            if (!string.IsNullOrWhiteSpace(actor.CreatedByUserId))
                candidateIds.Add(actor.CreatedByUserId);

            foreach (var id in await GetPatientRelatedProviderIdsAsync(actorUserId, cancellationToken))
                candidateIds.Add(id);
        }

        if (isLabPartner)
        {
            foreach (var id in await GetCreatedUserIdsAsync(actorUserId, cancellationToken))
                candidateIds.Add(id);

            foreach (var id in await GetLabRelatedPatientIdsAsync(actorUserId, cancellationToken))
                candidateIds.Add(id);

            foreach (var id in await GetActiveUserIdsInRoleAsync(UserRoles.Doctor, cancellationToken))
                candidateIds.Add(id);
        }

        if (isDoctor)
        {
            foreach (var id in await GetCreatedUserIdsAsync(actorUserId, cancellationToken))
                candidateIds.Add(id);

            foreach (var id in await GetDoctorRelatedPatientIdsAsync(actorUserId, cancellationToken))
                candidateIds.Add(id);

            foreach (var id in await GetActiveUserIdsInRoleAsync(UserRoles.LabPartner, cancellationToken))
                candidateIds.Add(id);
        }

        candidateIds.Remove(actorUserId);
        return await FilterToActiveUserIdsAsync(candidateIds, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> FilterToPeersActorMayChatAsync(
        string actorUserId,
        IReadOnlyCollection<string> candidateUserIds,
        CancellationToken cancellationToken = default)
    {
        var allowedSet = (await GetPeerUserIdsActorMayChatAsync(actorUserId, cancellationToken))
            .ToHashSet(StringComparer.Ordinal);

        var distinct = candidateUserIds
            .Where(id => !string.Equals(id, actorUserId, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        return distinct.Where(allowedSet.Contains).ToList();
    }

    private async Task<User> RequireActiveUserAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        if (!user.IsActive)
            throw new ApplicationForbiddenException("Your account is inactive.");

        return user;
    }

    private async Task RequireActivePeerAsync(string peerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var peer = await _userManager.FindByIdAsync(peerId)
            ?? throw new ApplicationBadRequestException($"User '{peerId}' was not found.");

        if (!peer.IsActive)
            throw new ApplicationBadRequestException($"User '{peerId}' is not active.");
    }

    private async Task<IReadOnlyList<string>> GetActiveUserIdsInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        return await (
            from user in _db.Users.AsNoTracking()
            join userRole in _db.UserRoles.AsNoTracking() on user.Id equals userRole.UserId
            join role in _db.Roles.AsNoTracking() on userRole.RoleId equals role.Id
            where user.IsActive && role.Name == roleName
            orderby user.FullName
            select user.Id)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<string>> GetCreatedUserIdsAsync(string actorUserId, CancellationToken cancellationToken)
    {
        return await _db.Users.AsNoTracking()
            .Where(user => user.IsActive && user.CreatedByUserId == actorUserId)
            .OrderBy(user => user.FullName)
            .Select(user => user.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<string>> GetDoctorRelatedPatientIdsAsync(string doctorUserId, CancellationToken cancellationToken)
    {
        var directPatientIds = await _db.TestRequests.AsNoTracking()
            .Where(request => request.DoctorId == doctorUserId && request.DirectPatientId != null)
            .Select(request => request.DirectPatientId!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return directPatientIds.Distinct(StringComparer.Ordinal).ToList();
    }

    private async Task<IReadOnlyList<string>> GetLabRelatedPatientIdsAsync(string labUserId, CancellationToken cancellationToken)
    {
        var directPatientIds = await _db.TestRequests.AsNoTracking()
            .Where(request => request.LabClientId == labUserId && request.DirectPatientId != null)
            .Select(request => request.DirectPatientId!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return directPatientIds.Distinct(StringComparer.Ordinal).ToList();
    }

    private async Task<IReadOnlyList<string>> GetPatientRelatedProviderIdsAsync(string patientUserId, CancellationToken cancellationToken)
    {
        var directDoctorIds = await _db.TestRequests.AsNoTracking()
            .Where(request => request.DirectPatientId == patientUserId)
            .Where(request => request.DoctorId != null)
            .Select(request => request.DoctorId!)
            .Distinct()
            .ToListAsync(cancellationToken);

        var directLabIds = await _db.TestRequests.AsNoTracking()
            .Where(request => request.DirectPatientId == patientUserId)
            .Where(request => request.LabClientId != null)
            .Select(request => request.LabClientId!)
            .Distinct()
            .ToListAsync(cancellationToken);

        var externalDoctorIds = await (
            from request in _db.TestRequests.AsNoTracking()
            join externalPatient in _db.ExternalPatients.AsNoTracking()
                on request.ExternalPatientId equals externalPatient.Id
            where externalPatient.LinkedDirectPatientId == patientUserId
            where request.DoctorId != null
            select request.DoctorId!)
            .Distinct()
            .ToListAsync(cancellationToken);

        var externalLabIds = await (
            from request in _db.TestRequests.AsNoTracking()
            join externalPatient in _db.ExternalPatients.AsNoTracking()
                on request.ExternalPatientId equals externalPatient.Id
            where externalPatient.LinkedDirectPatientId == patientUserId
            where request.LabClientId != null
            select request.LabClientId!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return directDoctorIds
            .Concat(directLabIds)
            .Concat(externalDoctorIds)
            .Concat(externalLabIds)
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private async Task<IReadOnlyList<string>> FilterToActiveUserIdsAsync(
        IEnumerable<string> userIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = userIds
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (distinctIds.Length == 0)
            return [];

        return await _db.Users.AsNoTracking()
            .Where(user => user.IsActive && distinctIds.Contains(user.Id))
            .OrderBy(user => user.FullName)
            .Select(user => user.Id)
            .ToListAsync(cancellationToken);
    }
}
