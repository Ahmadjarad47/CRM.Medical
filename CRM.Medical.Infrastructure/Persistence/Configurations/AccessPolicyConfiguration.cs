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

        builder.Property(x => x.Resource)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Effect)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.SubjectType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.SubjectId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Condition)
            .HasColumnType("jsonb");

        builder.Property(x => x.Description)
            .HasMaxLength(512);

        builder.HasIndex(x => new { x.Resource, x.Action, x.SubjectType, x.SubjectId, x.IsEnabled });
        builder.HasIndex(x => new { x.Priority, x.Effect });
    }
}
