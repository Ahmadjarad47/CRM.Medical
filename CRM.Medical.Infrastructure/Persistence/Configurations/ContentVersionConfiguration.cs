using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class ContentVersionConfiguration : IEntityTypeConfiguration<ContentVersion>
{
    public void Configure(EntityTypeBuilder<ContentVersion> builder)
    {
        builder.ToTable("content_versions");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(v => v.SnapshotData)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.ChangeNotes)
            .HasMaxLength(2000);

        builder.Property(v => v.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.HasIndex(v => v.PageId);
        builder.HasIndex(v => new { v.PageId, v.VersionNumber }).IsUnique();
        builder.HasIndex(v => v.CreatedAt);
    }
}
