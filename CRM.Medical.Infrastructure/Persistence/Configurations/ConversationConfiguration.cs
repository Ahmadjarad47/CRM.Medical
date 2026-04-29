using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("conversations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Title)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.ConfigureAuditColumns();

        builder.HasIndex(c => c.CreatedByUserId);
        builder.HasIndex(c => c.CreatedAt);

        builder.HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
