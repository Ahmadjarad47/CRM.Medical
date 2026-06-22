using CRM.Medical.Domain.Constants;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("pages");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(p => p.TemplateKey)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.Order)
            .IsRequired()
            .HasColumnName("sort_order");

        builder.Property(p => p.PublishStatus)
            .IsRequired()
            .HasMaxLength(32)
            .HasDefaultValue(PagePublishStatuses.Draft);

        builder.Property(p => p.IsVisibleInNav)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(p => p.UpdatedByUserId)
            .HasMaxLength(450);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.HasOne(p => p.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Translations)
            .WithOne(t => t.Page)
            .HasForeignKey(t => t.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ContentBlocks)
            .WithOne(b => b.Page)
            .HasForeignKey(b => b.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Versions)
            .WithOne(v => v.Page)
            .HasForeignKey(v => v.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.TemplateKey).IsUnique();
        builder.HasIndex(p => p.ParentId);
        builder.HasIndex(p => p.PublishStatus);
        builder.HasIndex(p => p.IsVisibleInNav);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.CreatedAt);
    }
}
