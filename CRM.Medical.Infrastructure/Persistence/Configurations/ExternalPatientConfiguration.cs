using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class ExternalPatientConfiguration : IEntityTypeConfiguration<ExternalPatient>
{
    public void Configure(EntityTypeBuilder<ExternalPatient> builder)
    {
        builder.ToTable("external_patients");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.FullName).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Gender).IsRequired().HasMaxLength(64);
        builder.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(64);
        builder.Property(e => e.ExternalId).HasMaxLength(256);

        builder.Property(e => e.LinkedDirectPatientId).HasMaxLength(450);

        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.ExternalId);
        builder.HasIndex(e => e.LinkedDirectPatientId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.LinkedDirectPatientId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
