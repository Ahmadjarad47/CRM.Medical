using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Domain.Chat;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Chat;

public sealed class ChatPersistence(MedicalDbContext db) : IChatPersistence
{
    private readonly MedicalDbContext _db = db;

    public async Task<Conversation?> FindActiveDirectConversationBetweenAsync(
        string userIdA,
        string userIdB,
        CancellationToken cancellationToken = default)
    {
        var conversationId = await (
            from p1 in _db.ConversationParticipants.AsNoTracking()
                .Where(x => x.LeftAt == null && x.UserId == userIdA)
            join p2 in _db.ConversationParticipants.AsNoTracking()
                    .Where(x => x.LeftAt == null && x.UserId == userIdB)
                on p1.ConversationId equals p2.ConversationId
            join c in _db.Conversations.AsNoTracking() on p1.ConversationId equals c.Id
            where c.Type == ConversationType.Direct
            select c.Id).FirstOrDefaultAsync(cancellationToken);

        if (conversationId == default)
            return null;

        return await _db.Conversations
            .Include(c => c.Participants)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);
    }

    public Task<Conversation?> GetConversationWithParticipantsAsync(Guid conversationId, CancellationToken cancellationToken = default) =>
        _db.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);

    public async Task AddConversationAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await _db.Conversations.AddAsync(conversation, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);

    public Task<bool> IsActiveParticipantAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default) =>
        _db.ConversationParticipants.AnyAsync(
            p => p.ConversationId == conversationId && p.UserId == userId && p.LeftAt == null,
            cancellationToken);

    public async Task<IReadOnlyList<ConversationParticipant>> GetActiveParticipantsAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default) =>
        await _db.ConversationParticipants.AsNoTracking()
            .Where(p => p.ConversationId == conversationId && p.LeftAt == null)
            .ToListAsync(cancellationToken);

    public Task<Message?> GetMessageAsync(Guid messageId, CancellationToken cancellationToken = default) =>
        _db.Messages
            .AsNoTracking()
            .Include(m => m.Conversation)
            .Include(m => m.Attachments)
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

    public Task<Message?> GetTrackedMessageAsync(Guid messageId, CancellationToken cancellationToken = default) =>
        _db.Messages
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

    public Task<bool> HasUserReadMessageAsync(Guid messageId, string userId, CancellationToken cancellationToken = default) =>
        _db.MessageReads.AnyAsync(r => r.MessageId == messageId && r.UserId == userId, cancellationToken);

    public async Task AddMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _db.Messages.AddAsync(message, cancellationToken);
    }

    public Task<int> CountUnreadForUserAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default) =>
        _db.Messages.CountAsync(
            m => m.ConversationId == conversationId
                && m.SenderId != userId
                && !m.Reads.Any(r => r.UserId == userId),
            cancellationToken);

    public async Task<IReadOnlyList<Conversation>> ListConversationsForUserAsync(
        string userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query =
            from p in _db.ConversationParticipants.AsNoTracking()
            where p.UserId == userId && p.LeftAt == null
            join c in _db.Conversations.AsNoTracking() on p.ConversationId equals c.Id
            orderby c.CreatedAt descending
            select c;

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Message>> ListMessagesAsync(
        Guid conversationId,
        DateTime? beforeUtc,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Messages
            .AsNoTracking()
            .Include(m => m.Attachments)
            .Where(m => m.ConversationId == conversationId);

        if (beforeUtc is { } ts)
            query = query.Where(m => m.CreatedAt < ts);

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, Message?>> GetLastMessagesByConversationIdsAsync(
        IReadOnlyCollection<Guid> conversationIds,
        CancellationToken cancellationToken = default)
    {
        if (conversationIds.Count == 0)
            return new Dictionary<Guid, Message?>();

        // Per-conversation latest message — subquery for efficiency on large datasets
        var latest = await (
            from m in _db.Messages.AsNoTracking()
            where conversationIds.Contains(m.ConversationId)
            group m by m.ConversationId into g
            select new
            {
                ConversationId = g.Key,
                MaxCreated = g.Max(x => x.CreatedAt)
            }).ToListAsync(cancellationToken);

        var ids = latest
            .Select(x => x.ConversationId)
            .ToList();

        var maxMap = latest.ToDictionary(x => x.ConversationId, x => x.MaxCreated);

        var messages = await _db.Messages.AsNoTracking()
            .Where(m => ids.Contains(m.ConversationId))
            .Where(m => maxMap[m.ConversationId] == m.CreatedAt)
            .ToListAsync(cancellationToken);

        var dict = conversationIds.ToDictionary(id => id, _ => (Message?)null);
        foreach (var m in messages)
            dict[m.ConversationId] = m;

        return dict;
    }

    public async Task AddMessageReadAsync(MessageRead read, CancellationToken cancellationToken = default)
    {
        await _db.MessageReads.AddAsync(read, cancellationToken);
    }

    public Task<MessageAttachment?> GetAttachmentByIdAsync(Guid attachmentId, CancellationToken cancellationToken = default) =>
        _db.MessageAttachments
            .Include(a => a.Message)
            .FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken);

    public async Task AddMessageAttachmentAsync(MessageAttachment attachment, CancellationToken cancellationToken = default)
    {
        await _db.MessageAttachments.AddAsync(attachment, cancellationToken);
    }

    public Task<ConversationParticipant?> GetParticipantAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default) =>
        _db.ConversationParticipants.FirstOrDefaultAsync(
            p => p.UserId == userId && p.ConversationId == conversationId,
            cancellationToken);

    public Task AddParticipantsAsync(
        IReadOnlyCollection<ConversationParticipant> participants,
        CancellationToken cancellationToken = default) =>
        _db.ConversationParticipants.AddRangeAsync(participants, cancellationToken);

    public async Task LeaveConversationAsync(
        string userId,
        Guid conversationId,
        DateTime leftAtUtc,
        CancellationToken cancellationToken = default)
    {
        var participant = await _db.ConversationParticipants
            .FirstOrDefaultAsync(
                p => p.UserId == userId && p.ConversationId == conversationId && p.LeftAt == null,
                cancellationToken)
            ?? throw new ApplicationNotFoundException("Conversation participant was not found.");

        participant.LeftAt = leftAtUtc;
        participant.UpdatedAt = leftAtUtc;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
