using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class ContentBlockConfiguration : IEntityTypeConfiguration<ContentBlock>
{
    public void Configure(EntityTypeBuilder<ContentBlock> builder)
    {
        builder.ToTable("content_blocks");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(b => b.BlockType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Order)
            .IsRequired()
            .HasColumnName("sort_order");

        builder.Property(b => b.CustomCssClass)
            .HasMaxLength(200);

        builder.Property(b => b.CustomStyles)
            .HasColumnType("jsonb");

        builder.Property(b => b.Animation)
            .HasMaxLength(100);

        builder.Property(b => b.VisibilityRules)
            .HasColumnType("jsonb");

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt);

        builder.HasMany(b => b.Localizations)
            .WithOne(l => l.ContentBlock)
            .HasForeignKey(l => l.ContentBlockId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.PageId);
        builder.HasIndex(b => b.IsActive);
        builder.HasIndex(b => new { b.PageId, b.Order });
    }
}
