using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
{
    public void Configure(EntityTypeBuilder<Availability> builder)
    {
        builder.ToTable("availabilities");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.UserId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.DayOfWeek).IsRequired();
        builder.Property(e => e.StartTime).IsRequired();
        builder.Property(e => e.EndTime).IsRequired();
        builder.Property(e => e.SlotDuration).IsRequired();
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.DayOfWeek, e.IsActive });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Availability_DayOfWeek", "\"DayOfWeek\" >= 0 AND \"DayOfWeek\" <= 6");
            t.HasCheckConstraint("CK_Availability_SlotDuration", "\"SlotDuration\" > 0");
            t.HasCheckConstraint("CK_Availability_TimeRange", "\"StartTime\" < \"EndTime\"");
        });
    }
}
