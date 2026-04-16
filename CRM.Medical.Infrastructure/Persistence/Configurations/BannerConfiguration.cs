using System.Text.Json;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("banners");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.MediaUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(b => b.InternalLink)
            .HasMaxLength(2048);

        builder.Property(b => b.ExternalLink)
            .HasMaxLength(2048);

        builder.Property(b => b.TargetType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Location)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.DisplayOrder)
            .IsRequired();

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.VisibilityRules)
            .HasColumnType("jsonb");

        builder.Property(b => b.StartDate)
            .IsRequired();

        builder.Property(b => b.EndDate)
            .IsRequired();

        builder.Property(b => b.ViewsCount)
            .IsRequired();

        builder.Property(b => b.ClicksCount)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.HasIndex(b => b.IsActive);
        builder.HasIndex(b => b.Location);
        builder.HasIndex(b => b.StartDate);
        builder.HasIndex(b => b.EndDate);
        builder.HasIndex(b => b.DisplayOrder);
        builder.HasIndex(b => b.CreatedAt);
    }
}

