using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Chat;

public sealed class EfChatUserProfileReader(MedicalDbContext db) : IChatUserProfileReader
{
    private readonly MedicalDbContext _db = db;

    public async Task<IReadOnlyDictionary<string, ChatUserProfileSnapshot>> GetProfilesAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var distinct = userIds
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (distinct.Length == 0)
            return new Dictionary<string, ChatUserProfileSnapshot>(StringComparer.Ordinal);

        var users = await _db.Users.AsNoTracking()
            .Where(u => distinct.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName, u.Email, u.PhoneNumber })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var roleRows = await (
                from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                where distinct.Contains(ur.UserId)
                select new { ur.UserId, RoleName = r.Name })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var roleByUser = roleRows
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.RoleName).Where(static n => !string.IsNullOrWhiteSpace(n))
                    .OrderBy(static n => n, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault(),
                StringComparer.Ordinal);

        var dict = new Dictionary<string, ChatUserProfileSnapshot>(StringComparer.Ordinal);
        foreach (var u in users)
        {
            roleByUser.TryGetValue(u.Id, out var role);
            dict[u.Id] = new ChatUserProfileSnapshot(u.FullName, u.Email, u.PhoneNumber, role);
        }

        return dict;
    }
}
