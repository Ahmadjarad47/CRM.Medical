using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.City)
            .HasMaxLength(100);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatedByUserId)
            .HasMaxLength(450);

        builder.HasOne(u => u.CreatedBy)
            .WithMany()
            .HasForeignKey(u => u.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(u => u.CreatedByUserId);

        builder.Property(u => u.ProfileMetadata)
            .HasColumnType("jsonb");

        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.CreatedAt);
        builder.HasIndex(u => u.FullName);
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
