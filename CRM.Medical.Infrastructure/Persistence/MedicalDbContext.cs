using CRM.Medical.Application.Common.Time;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class MedicalDbContext(
    DbContextOptions<MedicalDbContext> options,
    IDateTimeProvider dateTimeProvider)
    : IdentityDbContext<User, IdentityRole, string>(options)
{
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Complaint> Complaints => Set<Complaint>();

    public DbSet<SubscriptionPackage> SubscriptionPackages => Set<SubscriptionPackage>();

    public DbSet<SlideCard> SlideCards => Set<SlideCard>();

    public DbSet<Banner> Banners => Set<Banner>();

    public DbSet<Template> Templates => Set<Template>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

    public DbSet<MedicalTest> MedicalTests => Set<MedicalTest>();

    public DbSet<TestRequest> TestRequests => Set<TestRequest>();

    public DbSet<TestResult> TestResults => Set<TestResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(MedicalDbContext).Assembly);

        // Use snake_case table names consistent with PostgreSQL conventions
        builder.Entity<User>().ToTable("users");
        builder.Entity<IdentityRole>().ToTable("roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditing();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditing();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditing()
    {
        var utc = _dateTimeProvider.UtcNow;

        foreach (var entry in ChangeTracker.Entries<User>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CreatedAt == default)
                        entry.Entity.CreatedAt = utc;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utc;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CreatedAt == default)
                        entry.Entity.CreatedAt = utc;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utc;
                    break;
            }
        }
    }
}
