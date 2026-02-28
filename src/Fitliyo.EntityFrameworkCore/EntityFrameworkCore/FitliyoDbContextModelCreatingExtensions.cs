using Fitliyo.Categories;
using Fitliyo.Messaging;
using Fitliyo.Notifications;
using Fitliyo.Orders;
using Fitliyo.Packages;
using Fitliyo.Reviews;
using Fitliyo.Subscriptions;
using Fitliyo.Trainers;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Fitliyo.EntityFrameworkCore;

public static class FitliyoDbContextModelCreatingExtensions
{
    public static void ConfigureFitliyo(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* TrainerProfile */
        builder.Entity<TrainerProfile>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "TrainerProfiles", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Slug).IsRequired().HasMaxLength(TrainerConsts.MaxSlugLength);
            b.Property(x => x.Bio).HasMaxLength(TrainerConsts.MaxBioLength);
            b.Property(x => x.City).HasMaxLength(TrainerConsts.MaxCityLength);
            b.Property(x => x.District).HasMaxLength(TrainerConsts.MaxDistrictLength);
            b.Property(x => x.VerificationBadge).HasMaxLength(TrainerConsts.MaxVerificationBadgeLength);
            b.Property(x => x.InstagramUrl).HasMaxLength(TrainerConsts.MaxInstagramUrlLength);
            b.Property(x => x.YoutubeUrl).HasMaxLength(TrainerConsts.MaxYoutubeUrlLength);
            b.Property(x => x.WebsiteUrl).HasMaxLength(TrainerConsts.MaxWebsiteUrlLength);
            b.Property(x => x.AverageRating).HasPrecision(3, 2);

            b.HasIndex(x => x.Slug).IsUnique();
            b.HasIndex(x => x.UserId).IsUnique();
            b.HasIndex(x => x.TrainerType);
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.City);
        });

        /* TrainerCertificate */
        builder.Entity<TrainerCertificate>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "TrainerCertificates", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.CertificateName).IsRequired().HasMaxLength(TrainerConsts.MaxCertificateNameLength);
            b.Property(x => x.IssuingOrganization).HasMaxLength(TrainerConsts.MaxIssuingOrganizationLength);
            b.Property(x => x.DocumentUrl).HasMaxLength(TrainerConsts.MaxDocumentUrlLength);

            b.HasIndex(x => x.TrainerProfileId);

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* TrainerGallery */
        builder.Entity<TrainerGallery>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "TrainerGalleries", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.MediaUrl).IsRequired().HasMaxLength(TrainerConsts.MaxMediaUrlLength);
            b.Property(x => x.ThumbnailUrl).HasMaxLength(TrainerConsts.MaxThumbnailUrlLength);
            b.Property(x => x.Caption).HasMaxLength(TrainerConsts.MaxCaptionLength);

            b.HasIndex(x => x.TrainerProfileId);

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* Category */
        builder.Entity<Category>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Categories", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(CategoryConsts.MaxNameLength);
            b.Property(x => x.Slug).IsRequired().HasMaxLength(CategoryConsts.MaxSlugLength);
            b.Property(x => x.IconUrl).HasMaxLength(CategoryConsts.MaxIconUrlLength);
            b.Property(x => x.Description).HasMaxLength(CategoryConsts.MaxDescriptionLength);

            b.HasIndex(x => x.Slug).IsUnique();
            b.HasIndex(x => x.ParentId);

            b.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* TrainerCategoryMapping */
        builder.Entity<TrainerCategoryMapping>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "TrainerCategoryMappings", FitliyoConsts.DbSchema);
            b.HasKey(x => new { x.TrainerProfileId, x.CategoryId });

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* ServicePackage */
        builder.Entity<ServicePackage>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "ServicePackages", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(PackageConsts.MaxTitleLength);
            b.Property(x => x.Description).HasMaxLength(PackageConsts.MaxDescriptionLength);
            b.Property(x => x.Price).HasPrecision(10, 2);
            b.Property(x => x.DiscountedPrice).HasPrecision(10, 2);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(PackageConsts.MaxCurrencyLength);
            b.Property(x => x.CancellationPolicy).HasMaxLength(PackageConsts.MaxCancellationPolicyLength);
            b.Property(x => x.AverageRating).HasPrecision(3, 2);

            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.PackageType);

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* PackageAvailabilitySchedule */
        builder.Entity<PackageAvailabilitySchedule>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "PackageAvailabilitySchedules", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasIndex(x => x.ServicePackageId);

            b.HasOne<ServicePackage>()
                .WithMany()
                .HasForeignKey(x => x.ServicePackageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* PackageUnavailableDate */
        builder.Entity<PackageUnavailableDate>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "PackageUnavailableDates", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Reason).HasMaxLength(PackageConsts.MaxReasonLength);

            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => new { x.TrainerProfileId, x.UnavailableDate }).IsUnique();

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* Order */
        builder.Entity<Order>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Orders", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.OrderNumber).IsRequired().HasMaxLength(OrderConsts.MaxOrderNumberLength);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(OrderConsts.MaxCurrencyLength);
            b.Property(x => x.CancellationReason).HasMaxLength(OrderConsts.MaxCancellationReasonLength);
            b.Property(x => x.Notes).HasMaxLength(OrderConsts.MaxNotesLength);
            b.Property(x => x.PaymentTransactionId).HasMaxLength(OrderConsts.MaxPaymentTransactionIdLength);
            b.Property(x => x.PaymentProvider).HasMaxLength(OrderConsts.MaxPaymentProviderLength);
            b.Property(x => x.UnitPrice).HasPrecision(10, 2);
            b.Property(x => x.TotalAmount).HasPrecision(10, 2);
            b.Property(x => x.DiscountAmount).HasPrecision(10, 2);
            b.Property(x => x.NetAmount).HasPrecision(10, 2);
            b.Property(x => x.CommissionAmount).HasPrecision(10, 2);
            b.Property(x => x.TrainerPayoutAmount).HasPrecision(10, 2);

            b.HasIndex(x => x.OrderNumber).IsUnique();
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => x.Status);

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<ServicePackage>()
                .WithMany()
                .HasForeignKey(x => x.ServicePackageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* Session */
        builder.Entity<Session>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Sessions", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.MeetingUrl).HasMaxLength(OrderConsts.MaxMeetingUrlLength);
            b.Property(x => x.TrainerNotes).HasMaxLength(OrderConsts.MaxSessionNotesLength);
            b.Property(x => x.StudentNotes).HasMaxLength(OrderConsts.MaxSessionNotesLength);

            b.HasIndex(x => x.OrderId);
            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => x.ScheduledStartTime);

            b.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* Review */
        builder.Entity<Review>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Reviews", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Comment).HasMaxLength(ReviewConsts.MaxCommentLength);
            b.Property(x => x.TrainerReply).HasMaxLength(ReviewConsts.MaxReplyLength);

            b.HasIndex(x => x.OrderId).IsUnique();
            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => x.Rating);

            b.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* Conversation */
        builder.Entity<Conversation>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Conversations", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasIndex(x => x.InitiatorId);
            b.HasIndex(x => x.ParticipantId);
            b.HasIndex(x => new { x.InitiatorId, x.ParticipantId }).IsUnique();
        });

        /* Message */
        builder.Entity<Message>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Messages", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Content).IsRequired().HasMaxLength(MessageConsts.MaxContentLength);
            b.Property(x => x.AttachmentUrl).HasMaxLength(MessageConsts.MaxAttachmentUrlLength);

            b.HasIndex(x => x.ConversationId);
            b.HasIndex(x => x.SenderId);
            b.HasIndex(x => new { x.ConversationId, x.IsRead });

            b.HasOne<Conversation>()
                .WithMany()
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* Notification */
        builder.Entity<Notification>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Notifications", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(NotificationConsts.MaxTitleLength);
            b.Property(x => x.Body).HasMaxLength(NotificationConsts.MaxBodyLength);
            b.Property(x => x.ActionUrl).HasMaxLength(NotificationConsts.MaxActionUrlLength);
            b.Property(x => x.DataJson).HasMaxLength(NotificationConsts.MaxDataJsonLength);

            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.UserId, x.IsRead });
            b.HasIndex(x => x.NotificationType);
        });

        /* SubscriptionPlan */
        builder.Entity<SubscriptionPlan>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "SubscriptionPlans", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(SubscriptionConsts.MaxPlanNameLength);
            b.Property(x => x.Description).HasMaxLength(SubscriptionConsts.MaxPlanDescriptionLength);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(SubscriptionConsts.MaxCurrencyLength);
            b.Property(x => x.FeaturesJson).HasMaxLength(SubscriptionConsts.MaxFeaturesJsonLength);

            b.Property(x => x.Price).HasPrecision(18, 2);
            b.Property(x => x.CommissionRate).HasPrecision(5, 4);

            b.HasIndex(x => x.Tier);
            b.HasIndex(x => x.IsActive);
        });

        /* TrainerSubscription */
        builder.Entity<TrainerSubscription>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "TrainerSubscriptions", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.PaymentReference).HasMaxLength(SubscriptionConsts.MaxPaymentReferenceLength);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(SubscriptionConsts.MaxCurrencyLength);
            b.Property(x => x.PaidAmount).HasPrecision(18, 2);

            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => x.SubscriptionPlanId);
            b.HasIndex(x => new { x.TrainerProfileId, x.Status });

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<SubscriptionPlan>()
                .WithMany()
                .HasForeignKey(x => x.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
