using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Users.Services;

/// <summary>
/// Enforces who may manage users via admin APIs: Admin (all users); Doctor/Lab (users they created, or patients linked via appointments).
/// </summary>
public interface IUserManagementAccess
{
    /// <summary>
    /// Ensures the caller may use user-creation APIs (Admin, Doctor, or Lab partner).
    /// </summary>
    Task EnsureActorCanCreateUsersAsync(
        string actorUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Throws if the actor may not manage the target user.
    /// </summary>
    Task EnsureActorCanManageUserAsync(
        string actorUserId,
        User targetUser,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the query unchanged for administrators. For doctors/labs, restricts to created users or patients with at least one relevant appointment.
    /// </summary>
    Task<IQueryable<User>> ScopeUsersQueryForActorAsync(
        IQueryable<User> users,
        string actorUserId,
        CancellationToken cancellationToken = default);
}
