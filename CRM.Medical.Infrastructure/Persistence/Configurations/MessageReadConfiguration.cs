using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class MessageReadConfiguration : IEntityTypeConfiguration<MessageRead>
{
    public void Configure(EntityTypeBuilder<MessageRead> builder)
    {
        builder.ToTable("message_reads");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(r => r.ReadAt)
            .IsRequired();

        builder.ConfigureAuditColumns();

        builder.HasIndex(r => new { r.MessageId, r.UserId })
            .IsUnique();

        builder.HasOne(r => r.Message)
            .WithMany(m => m.Reads)
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
