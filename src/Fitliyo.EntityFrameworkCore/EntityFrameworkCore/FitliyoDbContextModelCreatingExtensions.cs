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

        /* UserProfile */
        builder.Entity<UserProfile>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "UserProfiles", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.BloodType).HasMaxLength(UserProfileConsts.MaxBloodTypeLength);
            b.Property(x => x.ChronicConditions).HasMaxLength(UserProfileConsts.MaxChronicConditionsLength);
            b.Property(x => x.Allergies).HasMaxLength(UserProfileConsts.MaxAllergiesLength);
            b.Property(x => x.Medications).HasMaxLength(UserProfileConsts.MaxMedicationsLength);
            b.Property(x => x.Injuries).HasMaxLength(UserProfileConsts.MaxInjuriesLength);
            b.Property(x => x.EmergencyContact).HasMaxLength(UserProfileConsts.MaxEmergencyContactLength);
            b.Property(x => x.Phone).HasMaxLength(UserProfileConsts.MaxPhoneLength);
            b.Property(x => x.Notes).HasMaxLength(UserProfileConsts.MaxNotesLength);
            b.Property(x => x.DoctorNotes).HasMaxLength(UserProfileConsts.MaxDoctorNotesLength);
            b.Property(x => x.AlcoholConsumption).HasMaxLength(UserProfileConsts.MaxAlcoholConsumptionLength);
            b.HasIndex(x => x.UserId).IsUnique();
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
            b.HasIndex(x => x.PaymentId);

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
            b.Property(x => x.Location).HasMaxLength(OrderConsts.MaxLocationLength);

            b.HasIndex(x => x.OrderId);
            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => x.ScheduledStartTime);
            b.HasIndex(x => x.RescheduledFromSessionId);

            b.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Session>()
                .WithMany()
                .HasForeignKey(x => x.RescheduledFromSessionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* Review */
        builder.Entity<Review>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Reviews", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Comment).HasMaxLength(ReviewConsts.MaxCommentLength);
            b.Property(x => x.TrainerReply).HasMaxLength(ReviewConsts.MaxReplyLength);
            b.Property(x => x.OverallRating).HasPrecision(3, 2);

            b.HasIndex(x => x.OrderId).IsUnique();
            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => x.Rating);
            b.HasIndex(x => x.ServicePackageId);

            b.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<ServicePackage>()
                .WithMany()
                .HasForeignKey(x => x.ServicePackageId)
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

        /* Payment */
        builder.Entity<Payment>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Payments", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.ProviderPaymentId).IsRequired().HasMaxLength(PaymentConsts.MaxProviderPaymentIdLength);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(PaymentConsts.MaxCurrencyLength);
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.RefundAmount).HasPrecision(18, 2);
            b.Property(x => x.ReceiptUrl).HasMaxLength(PaymentConsts.MaxReceiptUrlLength);
            b.Property(x => x.CardLastFour).HasMaxLength(PaymentConsts.MaxCardLastFourLength);
            b.Property(x => x.ProviderResponse).HasMaxLength(PaymentConsts.MaxProviderResponseLength);

            b.HasIndex(x => x.OrderId).IsUnique();
            b.HasIndex(x => x.ProviderPaymentId);
            b.HasIndex(x => x.Status);

            b.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* TrainerWallet */
        builder.Entity<TrainerWallet>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "TrainerWallets", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.AvailableBalance).HasPrecision(18, 2);
            b.Property(x => x.PendingBalance).HasPrecision(18, 2);
            b.Property(x => x.TotalEarned).HasPrecision(18, 2);
            b.Property(x => x.TotalWithdrawn).HasPrecision(18, 2);

            b.HasIndex(x => x.TrainerProfileId).IsUnique();

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* WalletTransaction */
        builder.Entity<WalletTransaction>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "WalletTransactions", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Description).IsRequired().HasMaxLength(WalletConsts.MaxDescriptionLength);
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.BalanceAfter).HasPrecision(18, 2);

            b.HasIndex(x => x.TrainerWalletId);
            b.HasIndex(x => x.TransactionType);

            b.HasOne<TrainerWallet>()
                .WithMany()
                .HasForeignKey(x => x.TrainerWalletId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* WithdrawalRequest */
        builder.Entity<WithdrawalRequest>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "WithdrawalRequests", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.Iban).IsRequired().HasMaxLength(WithdrawalConsts.MaxIbanLength);
            b.Property(x => x.AccountHolderName).IsRequired().HasMaxLength(WithdrawalConsts.MaxAccountHolderNameLength);
            b.Property(x => x.AdminNote).HasMaxLength(WithdrawalConsts.MaxAdminNoteLength);

            b.HasIndex(x => x.TrainerWalletId);
            b.HasIndex(x => x.Status);

            b.HasOne<TrainerWallet>()
                .WithMany()
                .HasForeignKey(x => x.TrainerWalletId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* ReviewHelpfulVote */
        builder.Entity<ReviewHelpfulVote>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "ReviewHelpfulVotes", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasIndex(x => x.ReviewId);
            b.HasIndex(x => x.VoterUserId);
            b.HasIndex(x => new { x.ReviewId, x.VoterUserId }).IsUnique();

            b.HasOne<Review>()
                .WithMany()
                .HasForeignKey(x => x.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* SupportTicket */
        builder.Entity<SupportTicket>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "SupportTickets", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Subject).IsRequired().HasMaxLength(SupportTicketConsts.MaxSubjectLength);
            b.Property(x => x.Message).IsRequired().HasMaxLength(SupportTicketConsts.MaxMessageLength);
            b.Property(x => x.AdminReply).HasMaxLength(SupportTicketConsts.MaxAdminReplyLength);

            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.Category);
        });

        /* FeaturedListing */
        builder.Entity<FeaturedListing>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "FeaturedListings", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.AdminNote).HasMaxLength(FeaturedListingConsts.MaxNoteLength);

            b.HasIndex(x => x.PageType);
            b.HasIndex(x => x.TrainerProfileId);
            b.HasIndex(x => x.ServicePackageId);
            b.HasIndex(x => x.IsActive);

            b.HasOne<TrainerProfile>()
                .WithMany()
                .HasForeignKey(x => x.TrainerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<ServicePackage>()
                .WithMany()
                .HasForeignKey(x => x.ServicePackageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* Dispute */
        builder.Entity<Dispute>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "Disputes", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Description).IsRequired().HasMaxLength(DisputeConsts.MaxDescriptionLength);
            b.Property(x => x.ResolutionNote).HasMaxLength(DisputeConsts.MaxResolutionNoteLength);

            b.HasIndex(x => x.OrderId);
            b.HasIndex(x => x.Status);

            b.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* BlogPost */
        builder.Entity<BlogPost>(b =>
        {
            b.ToTable(FitliyoConsts.DbTablePrefix + "BlogPosts", FitliyoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(BlogPostConsts.MaxTitleLength);
            b.Property(x => x.Slug).IsRequired().HasMaxLength(BlogPostConsts.MaxSlugLength);
            b.Property(x => x.Summary).HasMaxLength(BlogPostConsts.MaxSummaryLength);
            b.Property(x => x.Body).IsRequired().HasMaxLength(BlogPostConsts.MaxBodyLength);
            b.Property(x => x.AuthorName).HasMaxLength(BlogPostConsts.MaxAuthorNameLength);
            b.Property(x => x.FeaturedImageUrl).HasMaxLength(BlogPostConsts.MaxFeaturedImageUrlLength);

            b.HasIndex(x => x.Slug).IsUnique();
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.PublishedAt);
        });
    }
}
