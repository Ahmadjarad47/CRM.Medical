using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionPackageConfiguration : IEntityTypeConfiguration<SubscriptionPackage>
{
    public void Configure(EntityTypeBuilder<SubscriptionPackage> builder)
    {
        builder.ToTable("subscription_packages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.ValidityDays)
            .IsRequired();

        builder.Property(p => p.IncludedTests)
            .HasColumnType("jsonb");

        builder.Property(p => p.TargetAudience)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasIndex(p => p.TargetAudience);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.CreatedAt);
    }
}
