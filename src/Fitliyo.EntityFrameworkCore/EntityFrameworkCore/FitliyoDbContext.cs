using Fitliyo.Admin;
using Fitliyo.Categories;
using Fitliyo.Content;
using Fitliyo.Messaging;
using Fitliyo.Notifications;
using Fitliyo.Orders;
using Fitliyo.Payments;
using Fitliyo.ServicePackages;
using Fitliyo.Reviews;
using Fitliyo.Subscriptions;
using Fitliyo.Support;
using Fitliyo.Trainers;
using Fitliyo.Profiles;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Fitliyo.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class FitliyoDbContext :
    AbpDbContext<FitliyoDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    #region Marketplace Entities

    public DbSet<TrainerProfile> TrainerProfiles { get; set; }
    public DbSet<TrainerCertificate> TrainerCertificates { get; set; }
    public DbSet<TrainerGallery> TrainerGalleries { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<TrainerCategoryMapping> TrainerCategoryMappings { get; set; }
    public DbSet<ServicePackage> ServicePackages { get; set; }
    public DbSet<PackageAvailabilitySchedule> PackageAvailabilitySchedules { get; set; }
    public DbSet<PackageUnavailableDate> PackageUnavailableDates { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Session> CoachingSessions { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<TrainerSubscription> TrainerSubscriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<TrainerWallet> TrainerWallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
    public DbSet<ReviewHelpfulVote> ReviewHelpfulVotes { get; set; }
    public DbSet<SupportTicket> SupportTickets { get; set; }
    public DbSet<FeaturedListing> FeaturedListings { get; set; }
    public DbSet<Dispute> Disputes { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    #endregion

    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public FitliyoDbContext(DbContextOptions<FitliyoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        builder.ConfigureFitliyo();
    }
}
