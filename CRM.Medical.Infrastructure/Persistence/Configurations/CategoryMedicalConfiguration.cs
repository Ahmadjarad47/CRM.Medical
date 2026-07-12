using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class CategoryMedicalConfiguration : IEntityTypeConfiguration<CategoryMedical>
{
    public void Configure(EntityTypeBuilder<CategoryMedical> builder)
    {
        builder.ToTable("category_medical");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.NameAr).IsRequired().HasMaxLength(500);
        builder.Property(e => e.NameEn).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.ImageUrl).HasMaxLength(2048);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.DisplayOrder);

        builder.HasMany(e => e.MedicalTests)
            .WithOne(t => t.CategoryMedical)
            .HasForeignKey(t => t.CategoryMedicalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
