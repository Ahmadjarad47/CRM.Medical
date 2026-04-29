using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("message_attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(a => a.FileUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(a => a.FileType)
            .HasMaxLength(128);

        builder.Property(a => a.FileSize)
            .IsRequired();

        builder.Property(a => a.UploadedAt)
            .IsRequired();

        builder.ConfigureAuditColumns();

        builder.HasIndex(a => a.MessageId);

        builder.HasOne(a => a.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(a => a.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
