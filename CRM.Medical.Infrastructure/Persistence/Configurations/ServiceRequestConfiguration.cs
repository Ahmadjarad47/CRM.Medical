using CRM.Medical.Domain.Entities.ServiceRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class ServiceRequestPageSettingConfiguration : IEntityTypeConfiguration<ServiceRequestPageSetting>
{
    public void Configure(EntityTypeBuilder<ServiceRequestPageSetting> builder)
    {
        builder.ToTable("service_request_page_settings");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.PageType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(e => e.AnnouncementTextAr).IsRequired().HasMaxLength(500);
        builder.Property(e => e.AnnouncementTextEn).IsRequired().HasMaxLength(500);
        builder.Property(e => e.TitleAr).IsRequired().HasMaxLength(300);
        builder.Property(e => e.TitleEn).IsRequired().HasMaxLength(300);
        builder.Property(e => e.DescriptionAr).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.DescriptionEn).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.PageType).IsUnique();
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.CreatedAt);
    }
}

public sealed class VacantJobConfiguration : IEntityTypeConfiguration<VacantJob>
{
    public void Configure(EntityTypeBuilder<VacantJob> builder)
    {
        builder.ToTable("vacant_jobs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.TitleAr).IsRequired().HasMaxLength(300);
        builder.Property(e => e.TitleEn).IsRequired().HasMaxLength(300);
        builder.Property(e => e.DescriptionAr).HasMaxLength(4000);
        builder.Property(e => e.DescriptionEn).HasMaxLength(4000);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.SortOrder);
        builder.HasIndex(e => e.CreatedAt);
    }
}

public sealed class EmploymentApplicationRequestConfiguration : IEntityTypeConfiguration<EmploymentApplicationRequest>
{
    public void Configure(EntityTypeBuilder<EmploymentApplicationRequest> builder)
    {
        builder.ToTable("employment_application_requests");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.FullName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.ResidencePlace).IsRequired().HasMaxLength(300);
        builder.Property(e => e.MobileNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.Property(e => e.AcademicDegree).IsRequired().HasMaxLength(300);
        builder.Property(e => e.PreviousExperience).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.Skills).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.AdditionalCertificates).HasMaxLength(4000);
        builder.Property(e => e.CvFileUrl).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.CvOriginalFileName).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Status).IsRequired().HasMaxLength(64);
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.VacantJob)
            .WithMany(e => e.EmploymentApplications)
            .HasForeignKey(e => e.VacantJobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.VacantJobId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CreatedAt);
    }
}

public sealed class ClientJoinRequestConfiguration : IEntityTypeConfiguration<ClientJoinRequest>
{
    public void Configure(EntityTypeBuilder<ClientJoinRequest> builder)
    {
        builder.ToTable("client_join_requests");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.ManagerName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.LabName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.MobileNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Address).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.AdditionalInfo).HasMaxLength(4000);
        builder.Property(e => e.Status).IsRequired().HasMaxLength(64);
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CreatedAt);
    }
}

public sealed class ContractServiceRequestConfiguration : IEntityTypeConfiguration<ContractServiceRequest>
{
    public void Configure(EntityTypeBuilder<ContractServiceRequest> builder)
    {
        builder.ToTable("contract_service_requests");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.ContractType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(e => e.ResponsibleName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.OrganizationName).HasMaxLength(300);
        builder.Property(e => e.ContactNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Address).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.ContractDuration).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(e => e.AdditionalInfo).HasMaxLength(4000);
        builder.Property(e => e.Status).IsRequired().HasMaxLength(64);
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CreatedAt);
    }
}
