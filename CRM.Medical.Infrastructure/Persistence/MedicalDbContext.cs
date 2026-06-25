using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Entities.Accounting;
using CRM.Medical.Domain.Entities.Insurance;
using CRM.Medical.Domain.Entities.ServiceRequests;
using CRM.Medical.Domain.Entities.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class MedicalDbContext(
    DbContextOptions<MedicalDbContext> options,
    IDateTimeProvider dateTimeProvider,
    ICurrentUserAccessor currentUser)
    : IdentityDbContext<User, IdentityRole, string>(options)
{
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ICurrentUserAccessor _currentUser = currentUser;
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Complaint> Complaints => Set<Complaint>();

    public DbSet<SubscriptionPackage> SubscriptionPackages => Set<SubscriptionPackage>();

    public DbSet<SlideCard> SlideCards => Set<SlideCard>();

    public DbSet<Banner> Banners => Set<Banner>();

    public DbSet<Ad> Ads => Set<Ad>();

    public DbSet<WelcomePage> WelcomePages => Set<WelcomePage>();

    public DbSet<Page> Pages => Set<Page>();
    public DbSet<PageTranslation> PageTranslations => Set<PageTranslation>();
    public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();
    public DbSet<BlockLocalization> BlockLocalizations => Set<BlockLocalization>();
    public DbSet<ContentVersion> ContentVersions => Set<ContentVersion>();

    public DbSet<Template> Templates => Set<Template>();

    public DbSet<CategoryMedical> CategoryMedical => Set<CategoryMedical>();

    public DbSet<MedicalTest> MedicalTests => Set<MedicalTest>();

    public DbSet<TestRequest> TestRequests => Set<TestRequest>();

    public DbSet<ExternalPatient> ExternalPatients => Set<ExternalPatient>();

    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<MessageRead> MessageReads => Set<MessageRead>();

    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<AccessPolicy> AccessPolicies => Set<AccessPolicy>();
    public DbSet<UserDeviceToken> UserDeviceTokens => Set<UserDeviceToken>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StoreSetting> StoreSettings => Set<StoreSetting>();
    public DbSet<StoreBanner> StoreBanners => Set<StoreBanner>();
    public DbSet<StoreSlider> StoreSliders => Set<StoreSlider>();
    public DbSet<StoreSliderProduct> StoreSliderProducts => Set<StoreSliderProduct>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<StoreOrder> StoreOrders => Set<StoreOrder>();
    public DbSet<StoreOrderItem> StoreOrderItems => Set<StoreOrderItem>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<ServiceRequestPageSetting> ServiceRequestPageSettings => Set<ServiceRequestPageSetting>();
    public DbSet<VacantJob> VacantJobs => Set<VacantJob>();
    public DbSet<EmploymentApplicationRequest> EmploymentApplicationRequests => Set<EmploymentApplicationRequest>();
    public DbSet<ClientJoinRequest> ClientJoinRequests => Set<ClientJoinRequest>();
    public DbSet<ContractServiceRequest> ContractServiceRequests => Set<ContractServiceRequest>();
    public DbSet<AccountingPageSetting> AccountingPageSettings => Set<AccountingPageSetting>();
    public DbSet<LabAccountPayment> LabAccountPayments => Set<LabAccountPayment>();
    public DbSet<LabAccountStatementFile> LabAccountStatementFiles => Set<LabAccountStatementFile>();
    public DbSet<InsuranceApprovalRequest> InsuranceApprovalRequests => Set<InsuranceApprovalRequest>();

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
        var currentUserId = _currentUser.UserId;

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
                    if (string.IsNullOrWhiteSpace(entry.Entity.CreatedByUserId) && !string.IsNullOrWhiteSpace(currentUserId))
                        entry.Entity.CreatedByUserId = currentUserId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utc;
                    break;
            }
        }
    }
}
