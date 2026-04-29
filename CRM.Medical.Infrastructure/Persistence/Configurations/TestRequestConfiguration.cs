using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class TestRequestConfiguration : IEntityTypeConfiguration<TestRequest>
{
    public void Configure(EntityTypeBuilder<TestRequest> builder)
    {
        builder.ToTable("test_requests");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.RequestDate).IsRequired();
        builder.Property(e => e.Status).IsRequired().HasMaxLength(64);
        builder.Property(e => e.TotalAmount).IsRequired();
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.Property(e => e.Metadata).HasColumnType("jsonb");

        builder.Property(e => e.DoctorId).HasMaxLength(450);
        builder.Property(e => e.LabClientId).HasMaxLength(450);
        builder.Property(e => e.DirectPatientId).HasMaxLength(450);

        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.MedicalTestId);
        builder.HasIndex(e => e.DoctorId);
        builder.HasIndex(e => e.LabClientId);
        builder.HasIndex(e => e.DirectPatientId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CreatedByUserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.LabClientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.DirectPatientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.TestResult)
            .WithOne(r => r.TestRequest)
            .HasForeignKey<TestResult>(r => r.TestRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
