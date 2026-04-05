using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class MedicalTestConfiguration : IEntityTypeConfiguration<MedicalTest>
{
    public void Configure(EntityTypeBuilder<MedicalTest> builder)
    {
        builder.ToTable("medical_tests");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(t => t.NameAr)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.NameEn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Price)
            .IsRequired();

        builder.Property(t => t.Category)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.SampleType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.ParameterSchema)
            .HasColumnType("jsonb");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(t => t.CreatedByUserId)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Category);
        builder.HasIndex(t => t.CreatedByUserId);

        builder.HasOne(t => t.CreatedByUser)
            .WithMany(u => u.MedicalTestsCreated)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
