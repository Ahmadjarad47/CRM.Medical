using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class MedicalDbContext(DbContextOptions<MedicalDbContext> options)
    : IdentityDbContext<User, IdentityRole, string>(options)
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Complaint> Complaints => Set<Complaint>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<AppointmentType> AppointmentTypes => Set<AppointmentType>();

    public DbSet<SubscriptionPackage> SubscriptionPackages => Set<SubscriptionPackage>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<MedicalTest> MedicalTests => Set<MedicalTest>();

    public DbSet<TestRequest> TestRequests => Set<TestRequest>();

    public DbSet<TestResult> TestResults => Set<TestResult>();

    public DbSet<SlideCard> SlideCards => Set<SlideCard>();

    public DbSet<Banner> Banners => Set<Banner>();

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
}
