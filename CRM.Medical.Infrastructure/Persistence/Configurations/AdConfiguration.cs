using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class AdConfiguration : IEntityTypeConfiguration<Ad>
{
    public void Configure(EntityTypeBuilder<Ad> builder)
    {
        builder.ToTable("ads");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(a => a.MediaType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.DisplayMode)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.MediaUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(a => a.Latitude);

        builder.Property(a => a.Longitude);

        builder.Property(a => a.AddressName)
            .IsRequired()
            .HasMaxLength(300);

        builder.ConfigureAuditColumns();

        builder.HasIndex(a => a.MediaType);
        builder.HasIndex(a => a.DisplayMode);
        builder.HasIndex(a => a.CreatedAt);
    }
}
