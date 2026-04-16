using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class SlideCardConfiguration : IEntityTypeConfiguration<SlideCard>
{
    public void Configure(EntityTypeBuilder<SlideCard> builder)
    {
        builder.ToTable("slide_cards");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(s => s.ImageUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(s => s.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Discount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.ExpiryDate)
            .IsRequired();

        builder.Property(s => s.Badge)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.DetailPageLink)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(s => s.DisplayOrder)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => s.ExpiryDate);
        builder.HasIndex(s => s.DisplayOrder);
        builder.HasIndex(s => s.CreatedAt);
    }
}

