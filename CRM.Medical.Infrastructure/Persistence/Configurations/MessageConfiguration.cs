using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.SenderId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(m => m.Text)
            .HasMaxLength(8000);

        builder.Property(m => m.MessageType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.FileUrl)
            .HasMaxLength(2048);

        builder.Property(m => m.IsRead)
            .IsRequired();

        builder.ConfigureAuditColumns();

        builder.HasIndex(m => new { m.ConversationId, m.CreatedAt });

        builder.HasIndex(m => m.SenderId);

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ReplyTo)
            .WithMany(m => m.Replies)
            .HasForeignKey(m => m.ReplyToId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
