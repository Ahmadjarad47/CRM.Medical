using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class MedicalTestConfiguration : IEntityTypeConfiguration<MedicalTest>
{
    public void Configure(EntityTypeBuilder<MedicalTest> builder)
    {
        builder.ToTable("medical_tests");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.NameAr).IsRequired().HasMaxLength(500);
        builder.Property(e => e.NameEn).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Price).IsRequired();
        builder.Property(e => e.SampleType).IsRequired().HasMaxLength(200);

        builder.HasIndex(e => e.CategoryMedicalId);
        builder.Property(e => e.Status).HasConversion<string>().IsRequired().HasMaxLength(32);
        builder.Property(e => e.ParameterSchema).HasColumnType("jsonb");

        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.Status);
        builder.HasMany(e => e.TestRequests)
            .WithOne(r => r.MedicalTest)
            .HasForeignKey(r => r.MedicalTestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
