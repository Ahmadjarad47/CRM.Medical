using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class AccountingPageSettingConfiguration : IEntityTypeConfiguration<AccountingPageSetting>
{
    public void Configure(EntityTypeBuilder<AccountingPageSetting> builder)
    {
        builder.ToTable("accounting_page_settings");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.AnnouncementTextAr).IsRequired().HasMaxLength(500);
        builder.Property(e => e.AnnouncementTextEn).IsRequired().HasMaxLength(500);
        builder.Property(e => e.TitleAr).IsRequired().HasMaxLength(300);
        builder.Property(e => e.TitleEn).IsRequired().HasMaxLength(300);
        builder.Property(e => e.DescriptionAr).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.DescriptionEn).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.IsActive);
    }
}

public sealed class LabAccountPaymentConfiguration : IEntityTypeConfiguration<LabAccountPayment>
{
    public void Configure(EntityTypeBuilder<LabAccountPayment> builder)
    {
        builder.ToTable("lab_account_payments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.LabClientId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.PaidAt).IsRequired();
        builder.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(100);
        builder.Property(e => e.ReferenceNumber).HasMaxLength(200);
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.LabClient)
            .WithMany()
            .HasForeignKey(e => e.LabClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.LabClientId);
        builder.HasIndex(e => e.PaidAt);
        builder.HasIndex(e => new { e.LabClientId, e.PaidAt });
        builder.HasIndex(e => e.CreatedAt);
    }
}

public sealed class LabAccountStatementFileConfiguration : IEntityTypeConfiguration<LabAccountStatementFile>
{
    public void Configure(EntityTypeBuilder<LabAccountStatementFile> builder)
    {
        builder.ToTable("lab_account_statement_files");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.LabClientId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.PeriodFrom).IsRequired();
        builder.Property(e => e.PeriodTo).IsRequired();
        builder.Property(e => e.FileUrl).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.LabClient)
            .WithMany()
            .HasForeignKey(e => e.LabClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.LabClientId);
        builder.HasIndex(e => new { e.LabClientId, e.PeriodFrom, e.PeriodTo });
        builder.HasIndex(e => e.CreatedAt);
    }
}
