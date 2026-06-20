using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class WelcomePageConfiguration : IEntityTypeConfiguration<WelcomePage>
{
    public void Configure(EntityTypeBuilder<WelcomePage> builder)
    {
        builder.ToTable("welcome_pages");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(w => w.MediaType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(w => w.MediaUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.ConfigureAuditColumns();

        builder.HasIndex(w => w.IsActive);
        builder.HasIndex(w => w.CreatedAt);
    }
}
