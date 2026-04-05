using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.Notes)
            .HasMaxLength(4000);

        builder.Property(a => a.Slot)
            .IsRequired();

        builder.Property(a => a.LocationType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(a => a.AppointmentTypeId)
            .IsRequired();

        builder.Property(a => a.PatientId).IsRequired();
        builder.Property(a => a.CreatedByUserId).IsRequired();

        builder.HasIndex(a => a.PatientId);
        builder.HasIndex(a => a.DoctorId);
        builder.HasIndex(a => a.LabPartnerId);
        builder.HasIndex(a => a.Slot);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.AppointmentTypeId);
        builder.HasIndex(a => a.CreatedByUserId);

        builder.Property(a => a.MedicalTestCompletionStatus)
            .HasMaxLength(64);

        builder.HasIndex(a => a.MedicalTestId)
            .IsUnique();

        builder.HasIndex(a => a.MedicalTestCompletionStatus);

        builder.HasOne(a => a.MedicalTest)
            .WithOne(t => t.Appointment)
            .HasForeignKey<Appointment>(a => a.MedicalTestId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.AppointmentType)
            .WithMany(t => t.Appointments)
            .HasForeignKey(a => a.AppointmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
            .WithMany(u => u.AppointmentsAsPatient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
            .WithMany(u => u.AppointmentsAsDoctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.LabPartner)
            .WithMany(u => u.AppointmentsAsLabPartner)
            .HasForeignKey(a => a.LabPartnerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.CreatedByUser)
            .WithMany(u => u.AppointmentsCreated)
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
