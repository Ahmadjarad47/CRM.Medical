using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
{
    public void Configure(EntityTypeBuilder<TestResult> builder)
    {
        builder.ToTable("test_results");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(r => r.ResultDate)
            .IsRequired();

        builder.Property(r => r.ResultData)
            .HasColumnType("jsonb");

        builder.Property(r => r.PdfUrl)
            .HasMaxLength(2000);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(r => r.CreatedByUserId)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => r.TestRequestId)
            .IsUnique();

        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.ResultDate);
        builder.HasIndex(r => r.CreatedByUserId);

        builder.HasOne(r => r.TestRequest)
            .WithOne(q => q.Result)
            .HasForeignKey<TestResult>(r => r.TestRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.CreatedByUser)
            .WithMany(u => u.TestResultsCreated)
            .HasForeignKey(r => r.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
