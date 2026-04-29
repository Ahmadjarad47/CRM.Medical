using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
{
    public void Configure(EntityTypeBuilder<TestResult> builder)
    {
        builder.ToTable("test_results");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.ResultDate).IsRequired();
        builder.Property(e => e.Status).IsRequired().HasMaxLength(64);
        builder.Property(e => e.PdfUrl).HasMaxLength(2000);
        builder.Property(e => e.ResultData).HasColumnType("jsonb");

        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.TestRequestId).IsUnique();
        builder.HasIndex(e => e.Status);
    }
}
