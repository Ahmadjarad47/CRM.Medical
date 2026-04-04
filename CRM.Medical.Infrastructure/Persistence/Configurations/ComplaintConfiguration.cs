using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> builder)
    {
        builder.ToTable("complaints");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(8000);

        builder.Property(c => c.AttachmentUrl)
            .HasMaxLength(2048);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.CreatedAt);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Complaints)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
