using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Chat;

/// <summary>
/// Enforces healthcare chat routing rules using Identity roles and <see cref="TestRequest"/> workflow links.
/// </summary>
public sealed class ChatAuthorizationService(
    MedicalDbContext db,
    UserManager<User> userManager)
    : IChatAuthorizationService
{
    private readonly MedicalDbContext _db = db;
    private readonly UserManager<User> _userManager = userManager;

    public async Task EnsureActiveParticipantAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default)
    {
        var ok = await _db.ConversationParticipants.AnyAsync(
            p => p.ConversationId == conversationId && p.UserId == userId && p.LeftAt == null,
            cancellationToken);

        if (!ok)
            throw new ApplicationForbiddenException("You are not an active participant in this conversation.");
    }

    public async Task EnsureCanChatWithPeersAsync(
        string actorUserId,
        IReadOnlyCollection<string> otherUserIds,
        CancellationToken cancellationToken = default)
    {
        var actor = await _userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        var distinctPeers = otherUserIds
            .Where(id => !string.Equals(id, actorUserId, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        foreach (var peerId in distinctPeers)
        {
            var peer = await _userManager.FindByIdAsync(peerId)
                ?? throw new ApplicationBadRequestException($"User '{peerId}' was not found.");

            if (!peer.IsActive)
                throw new ApplicationBadRequestException($"User '{peerId}' is not active.");

            var allowed = await ArePeersAllowedAsync(actor, peer, cancellationToken);
            if (!allowed)
                throw new ApplicationForbiddenException(
                    "You are not allowed to start or join a conversation with one or more selected users.");
        }
    }

    public async Task<IReadOnlyList<string>> FilterToPeersActorMayChatAsync(
        string actorUserId,
        IReadOnlyCollection<string> candidateUserIds,
        CancellationToken cancellationToken = default)
    {
        var actor = await _userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        var distinct = candidateUserIds
            .Where(id => !string.Equals(id, actorUserId, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var allowed = new List<string>();
        foreach (var peerId in distinct)
        {
            var peer = await _userManager.FindByIdAsync(peerId);
            if (peer is null || !peer.IsActive)
                continue;

            if (await ArePeersAllowedAsync(actor, peer, cancellationToken))
                allowed.Add(peerId);
        }

        return allowed;
    }

    private async Task<bool> ArePeersAllowedAsync(User actor, User peer, CancellationToken cancellationToken)
    {
        if (string.Equals(actor.Id, peer.Id, StringComparison.Ordinal))
            return false;

        var actorRoles = await _userManager.GetRolesAsync(actor);
        var peerRoles = await _userManager.GetRolesAsync(peer);

        if (ContainsRole(actorRoles, UserRoles.Admin) || ContainsRole(peerRoles, UserRoles.Admin))
            return true;

        // Symmetric pairs — evaluate both orientations
        if (await PairDoctorPatientAsync(actor, peer, actorRoles, peerRoles, cancellationToken))
            return true;

        if (await PairDoctorLabAsync(actor, peer, actorRoles, peerRoles, cancellationToken))
            return true;

        if (await PairPatientLabAsync(actor, peer, actorRoles, peerRoles, cancellationToken))
            return true;

        return false;
    }

    private static bool ContainsRole(IList<string> roles, string role) =>
        roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

    private async Task<bool> PairDoctorPatientAsync(
        User a,
        User b,
        IList<string> rolesA,
        IList<string> rolesB,
        CancellationToken cancellationToken)
    {
        if (ContainsRole(rolesA, UserRoles.Doctor) && ContainsRole(rolesB, UserRoles.Patient))
            return await IsDoctorPatientAllowedAsync(a.Id, b.Id, cancellationToken);

        if (ContainsRole(rolesB, UserRoles.Doctor) && ContainsRole(rolesA, UserRoles.Patient))
            return await IsDoctorPatientAllowedAsync(b.Id, a.Id, cancellationToken);

        return false;
    }

    private async Task<bool> IsDoctorPatientAllowedAsync(string doctorId, string patientId, CancellationToken cancellationToken)
    {
        var patient = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == patientId, cancellationToken);
        if (patient is null)
            return false;

        if (string.Equals(patient.CreatedByUserId, doctorId, StringComparison.Ordinal))
            return true;

        var linkedDirectly = await _db.TestRequests.AsNoTracking().AnyAsync(
            r =>
                r.DirectPatientId == patientId
                && (r.DoctorId == doctorId || r.CreatedByUserId == doctorId),
            cancellationToken);
        if (linkedDirectly)
            return true;

        return await (
            from r in _db.TestRequests.AsNoTracking()
            join e in _db.ExternalPatients.AsNoTracking() on r.ExternalPatientId equals e.Id
            where e.LinkedDirectPatientId == patientId
                && (r.DoctorId == doctorId || r.CreatedByUserId == doctorId)
            select r.Id
        ).AnyAsync(cancellationToken);
    }

    private async Task<bool> PairDoctorLabAsync(
        User a,
        User b,
        IList<string> rolesA,
        IList<string> rolesB,
        CancellationToken cancellationToken)
    {
        if (ContainsRole(rolesA, UserRoles.Doctor) && ContainsRole(rolesB, UserRoles.LabPartner))
            return await DoctorLabLinkedAsync(a.Id, b.Id, cancellationToken);

        if (ContainsRole(rolesB, UserRoles.Doctor) && ContainsRole(rolesA, UserRoles.LabPartner))
            return await DoctorLabLinkedAsync(b.Id, a.Id, cancellationToken);

        return false;
    }

    private Task<bool> DoctorLabLinkedAsync(string doctorId, string labId, CancellationToken cancellationToken) =>
        _db.TestRequests.AsNoTracking().AnyAsync(
            r => r.DoctorId == doctorId && r.LabClientId == labId,
            cancellationToken);

    private async Task<bool> PairPatientLabAsync(
        User a,
        User b,
        IList<string> rolesA,
        IList<string> rolesB,
        CancellationToken cancellationToken)
    {
        if (ContainsRole(rolesA, UserRoles.Patient) && ContainsRole(rolesB, UserRoles.LabPartner))
            return await PatientLabLinkedAsync(a.Id, b.Id, cancellationToken);

        if (ContainsRole(rolesB, UserRoles.Patient) && ContainsRole(rolesA, UserRoles.LabPartner))
            return await PatientLabLinkedAsync(b.Id, a.Id, cancellationToken);

        return false;
    }

    private async Task<bool> PatientLabLinkedAsync(string patientId, string labId, CancellationToken cancellationToken)
    {
        var patient = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == patientId, cancellationToken);
        if (patient is null)
            return false;

        if (string.Equals(patient.CreatedByUserId, labId, StringComparison.Ordinal))
            return true;

        var linkedDirectly = await _db.TestRequests.AsNoTracking().AnyAsync(
            r =>
                r.DirectPatientId == patientId
                && r.LabClientId == labId,
            cancellationToken);
        if (linkedDirectly)
            return true;

        return await (
            from r in _db.TestRequests.AsNoTracking()
            join e in _db.ExternalPatients.AsNoTracking() on r.ExternalPatientId equals e.Id
            where e.LinkedDirectPatientId == patientId
                && r.LabClientId == labId
            select r.Id
        ).AnyAsync(cancellationToken);
    }
}
