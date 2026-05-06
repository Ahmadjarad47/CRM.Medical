using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class AccessPolicyConfiguration : IEntityTypeConfiguration<AccessPolicy>
{
    public void Configure(EntityTypeBuilder<AccessPolicy> builder)
    {
        builder.ToTable("access_policies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Resource).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Effect)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(x => x.SubjectType)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();
        builder.Property(x => x.SubjectKey).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Condition).HasColumnType("jsonb");
        builder.Property(x => x.Description).HasMaxLength(1024);
        builder.Property(x => x.UpdatedByUserId).HasMaxLength(450);

        builder.ConfigureAuditColumns();

        builder.HasIndex(x => new { x.Resource, x.Action, x.IsEnabled });
        builder.HasIndex(x => new { x.SubjectType, x.SubjectKey });
        builder.HasIndex(x => x.Priority);
    }
}
