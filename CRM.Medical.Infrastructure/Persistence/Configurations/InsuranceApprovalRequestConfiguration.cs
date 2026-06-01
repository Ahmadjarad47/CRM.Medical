using CRM.Medical.Domain.Entities.Insurance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class InsuranceApprovalRequestConfiguration : IEntityTypeConfiguration<InsuranceApprovalRequest>
{
    public void Configure(EntityTypeBuilder<InsuranceApprovalRequest> builder)
    {
        builder.ToTable("insurance_approval_requests");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.PatientId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.InsuredName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.InsuranceNumber).IsRequired().HasMaxLength(100);
        builder.Property(e => e.MobileNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.InsuranceCardImageUrl).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.InsuranceCardOriginalFileName).IsRequired().HasMaxLength(500);
        builder.Property(e => e.PrescriptionImageUrl).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.PrescriptionOriginalFileName).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.Property(e => e.RejectionReason).HasMaxLength(4000);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.InsuranceNumber);
        builder.HasIndex(e => e.CreatedAt);
    }
}
