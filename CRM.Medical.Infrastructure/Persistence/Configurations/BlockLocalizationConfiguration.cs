using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class BlockLocalizationConfiguration : IEntityTypeConfiguration<BlockLocalization>
{
    public void Configure(EntityTypeBuilder<BlockLocalization> builder)
    {
        builder.ToTable("block_localizations");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(l => l.Language)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(l => l.Heading)
            .HasMaxLength(300);

        builder.Property(l => l.Subheading)
            .HasMaxLength(600);

        builder.Property(l => l.Description)
            .HasMaxLength(4000);

        builder.Property(l => l.ContentData)
            .HasColumnType("jsonb");

        builder.Property(l => l.MediaUrl)
            .HasMaxLength(2048);

        builder.Property(l => l.MediaAltText)
            .HasMaxLength(500);

        builder.Property(l => l.ButtonText)
            .HasMaxLength(300);

        builder.Property(l => l.ButtonLink)
            .HasMaxLength(2048);

        builder.Property(l => l.ButtonStyle)
            .HasMaxLength(100);

        builder.HasIndex(l => l.ContentBlockId);
        builder.HasIndex(l => l.Language);
        builder.HasIndex(l => new { l.ContentBlockId, l.Language }).IsUnique();
    }
}
