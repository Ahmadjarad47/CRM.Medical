using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class PageTranslationConfiguration : IEntityTypeConfiguration<PageTranslation>
{
    public void Configure(EntityTypeBuilder<PageTranslation> builder)
    {
        builder.ToTable("page_translations");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(t => t.Language)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(t => t.MetaTitle)
            .HasMaxLength(300);

        builder.Property(t => t.MetaDescription)
            .HasMaxLength(1000);

        builder.Property(t => t.MetaKeywords)
            .HasMaxLength(1000);

        builder.Property(t => t.OpenGraphImageUrl)
            .HasMaxLength(2048);

        builder.Property(t => t.CanonicalUrl)
            .HasMaxLength(2048);

        builder.Property(t => t.BreadcrumbTitle)
            .HasMaxLength(300);

        builder.HasIndex(t => new { t.PageId, t.Language }).IsUnique();
        builder.HasIndex(t => new { t.Language, t.Slug }).IsUnique();
        builder.HasIndex(t => t.Slug);
    }
}
