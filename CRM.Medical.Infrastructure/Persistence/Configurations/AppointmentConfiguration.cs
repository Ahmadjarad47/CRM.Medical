using CRM.Medical.Domain.Constants;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.AvailabilityId);
        builder.Property(e => e.TestRequestId);
        builder.Property(e => e.ProviderUserId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.StartTime).IsRequired();
        builder.Property(e => e.EndTime).IsRequired();
        builder.Property(e => e.Status).IsRequired().HasMaxLength(64);
        builder.Property(e => e.PatientLocationType)
            .IsRequired()
            .HasMaxLength(32)
            .HasDefaultValue(AppointmentPatientLocationTypes.ComeToUs);
        builder.Property(e => e.Age);
        builder.Property(e => e.Gender).HasMaxLength(64);
        builder.Property(e => e.PatientLatitude);
        builder.Property(e => e.PatientLongitude);
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.Property(e => e.AttachmentUrl).HasMaxLength(2048);
        builder.Property(e => e.MedicalTestCompletionStatus).HasMaxLength(64);

        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.ProviderUserId);
        builder.HasIndex(e => e.AvailabilityId);
        builder.HasIndex(e => e.TestRequestId);
        builder.HasIndex(e => e.StartTime);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => new { e.ProviderUserId, e.StartTime, e.EndTime });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.ProviderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TestRequest>()
            .WithMany()
            .HasForeignKey(e => e.TestRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Availability>()
            .WithMany()
            .HasForeignKey(e => e.AvailabilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Appointment_TimeRange",
            "\"StartTime\" < \"EndTime\""));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Appointment_PatientLatitudeRange",
            "\"PatientLatitude\" IS NULL OR (\"PatientLatitude\" >= -90 AND \"PatientLatitude\" <= 90)"));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Appointment_PatientLongitudeRange",
            "\"PatientLongitude\" IS NULL OR (\"PatientLongitude\" >= -180 AND \"PatientLongitude\" <= 180)"));
    }
}
