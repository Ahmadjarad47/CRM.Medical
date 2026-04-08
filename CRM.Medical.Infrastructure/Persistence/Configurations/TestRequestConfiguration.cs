using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class TestRequestConfiguration : IEntityTypeConfiguration<TestRequest>
{
    public void Configure(EntityTypeBuilder<TestRequest> builder)
    {
        builder.ToTable("test_requests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(r => r.RequestDate)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(r => r.TotalAmount)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(4000);

        builder.Property(r => r.Metadata)
            .HasColumnType("jsonb");

        builder.Property(r => r.CreatedByUserId)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => r.MedicalTestId)
            .IsUnique();

        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.RequestDate);
        builder.HasIndex(r => r.CreatedByUserId);

        builder.HasOne(r => r.MedicalTest)
            .WithOne(t => t.TestRequest)
            .HasForeignKey<TestRequest>(r => r.MedicalTestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.CreatedByUser)
            .WithMany(u => u.TestRequestsCreated)
            .HasForeignKey(r => r.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
