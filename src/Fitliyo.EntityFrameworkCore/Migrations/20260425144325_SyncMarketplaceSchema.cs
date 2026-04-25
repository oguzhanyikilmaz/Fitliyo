using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitliyo.Migrations
{
    /// <inheritdoc />
    public partial class SyncMarketplaceSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId",
                table: "AppConversations");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "AppSessions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RescheduledFromSessionId",
                table: "AppSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommunicationRating",
                table: "AppReviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpertiseRating",
                table: "AppReviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HelpfulCount",
                table: "AppReviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "AppReviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedPurchase",
                table: "AppReviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OverallRating",
                table: "AppReviews",
                type: "numeric(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PunctualityRating",
                table: "AppReviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ServicePackageId",
                table: "AppReviews",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValueForMoneyRating",
                table: "AppReviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "AppOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgramAttachmentUrl",
                table: "AppOrders",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProgramDeliveredAt",
                table: "AppOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentFormData",
                table: "AppOrders",
                type: "character varying(8000)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StudentFormSubmittedAt",
                table: "AppOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrainerProgramNotes",
                table: "AppOrders",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "AppConversations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppBlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Body = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    AuthorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    FeaturedImageUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBlogPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppDisputes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    OpenedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisputeType = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResolutionNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ResolvedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDisputes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDisputes_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppFeaturedListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PageType = table.Column<int>(type: "integer", nullable: false),
                    TrainerProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServicePackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AdminNote = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFeaturedListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppFeaturedListings_AppServicePackages_ServicePackageId",
                        column: x => x.ServicePackageId,
                        principalTable: "AppServicePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppFeaturedListings_AppTrainerProfiles_TrainerProfileId",
                        column: x => x.TrainerProfileId,
                        principalTable: "AppTrainerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationType = table.Column<int>(type: "integer", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Body = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ActionUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    DataJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEmailSent = table.Column<bool>(type: "boolean", nullable: false),
                    IsPushSent = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentProvider = table.Column<int>(type: "integer", nullable: false),
                    ProviderPaymentId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    CardLastFour = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    ReceiptUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    RefundAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProviderResponse = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPayments_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppReviewHelpfulVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    VoterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsHelpful = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppReviewHelpfulVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppReviewHelpfulVotes_AppReviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "AppReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppSubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    PlanType = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    MaxPackageCount = table.Column<int>(type: "integer", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    HasFeaturedListing = table.Column<bool>(type: "boolean", nullable: false),
                    HasPrioritySupport = table.Column<bool>(type: "boolean", nullable: false),
                    HasAdvancedAnalytics = table.Column<bool>(type: "boolean", nullable: false),
                    FeaturesJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppSupportTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    AdminReply = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AdminReplyDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedToUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSupportTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppTrainerWallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PendingBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEarned = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalWithdrawn = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    LastPayoutAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTrainerWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTrainerWallets_AppTrainerProfiles_TrainerProfileId",
                        column: x => x.TrainerProfileId,
                        principalTable: "AppTrainerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppUserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    HeightCm = table.Column<decimal>(type: "numeric", nullable: true),
                    WeightKg = table.Column<decimal>(type: "numeric", nullable: true),
                    BloodType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ActivityLevel = table.Column<int>(type: "integer", nullable: false),
                    FitnessGoal = table.Column<int>(type: "integer", nullable: false),
                    ChronicConditions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Allergies = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Medications = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Injuries = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EmergencyContact = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    WaistCm = table.Column<decimal>(type: "numeric", nullable: true),
                    HipCm = table.Column<decimal>(type: "numeric", nullable: true),
                    NeckCm = table.Column<decimal>(type: "numeric", nullable: true),
                    TargetWeightKg = table.Column<decimal>(type: "numeric", nullable: true),
                    SleepHoursPerNight = table.Column<int>(type: "integer", nullable: true),
                    Smoking = table.Column<bool>(type: "boolean", nullable: true),
                    AlcoholConsumption = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RestingHeartRate = table.Column<int>(type: "integer", nullable: true),
                    DoctorNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppTrainerSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentReference = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTrainerSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTrainerSubscriptions_AppSubscriptionPlans_SubscriptionPl~",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "AppSubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppTrainerSubscriptions_AppTrainerProfiles_TrainerProfileId",
                        column: x => x.TrainerProfileId,
                        principalTable: "AppTrainerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppWalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerWalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    BalanceAfter = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWalletTransactions_AppTrainerWallets_TrainerWalletId",
                        column: x => x.TrainerWalletId,
                        principalTable: "AppTrainerWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppWithdrawalRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerWalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Iban = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    AccountHolderName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AdminNote = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWithdrawalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWithdrawalRequests_AppTrainerWallets_TrainerWalletId",
                        column: x => x.TrainerWalletId,
                        principalTable: "AppTrainerWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSessions_RescheduledFromSessionId",
                table: "AppSessions",
                column: "RescheduledFromSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppReviews_ServicePackageId",
                table: "AppReviews",
                column: "ServicePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_PaymentId",
                table: "AppOrders",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId_OrderId",
                table: "AppConversations",
                columns: new[] { "InitiatorId", "ParticipantId", "OrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_OrderId",
                table: "AppConversations",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogPosts_PublishedAt",
                table: "AppBlogPosts",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogPosts_Slug",
                table: "AppBlogPosts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogPosts_Status",
                table: "AppBlogPosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppDisputes_OrderId",
                table: "AppDisputes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDisputes_Status",
                table: "AppDisputes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppFeaturedListings_IsActive",
                table: "AppFeaturedListings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AppFeaturedListings_PageType",
                table: "AppFeaturedListings",
                column: "PageType");

            migrationBuilder.CreateIndex(
                name: "IX_AppFeaturedListings_ServicePackageId",
                table: "AppFeaturedListings",
                column: "ServicePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_AppFeaturedListings_TrainerProfileId",
                table: "AppFeaturedListings",
                column: "TrainerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AppNotifications_NotificationType",
                table: "AppNotifications",
                column: "NotificationType");

            migrationBuilder.CreateIndex(
                name: "IX_AppNotifications_UserId",
                table: "AppNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppNotifications_UserId_IsRead",
                table: "AppNotifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_AppPayments_OrderId",
                table: "AppPayments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppPayments_ProviderPaymentId",
                table: "AppPayments",
                column: "ProviderPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPayments_Status",
                table: "AppPayments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppReviewHelpfulVotes_ReviewId",
                table: "AppReviewHelpfulVotes",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_AppReviewHelpfulVotes_ReviewId_VoterUserId",
                table: "AppReviewHelpfulVotes",
                columns: new[] { "ReviewId", "VoterUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppReviewHelpfulVotes_VoterUserId",
                table: "AppReviewHelpfulVotes",
                column: "VoterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubscriptionPlans_IsActive",
                table: "AppSubscriptionPlans",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubscriptionPlans_Tier",
                table: "AppSubscriptionPlans",
                column: "Tier");

            migrationBuilder.CreateIndex(
                name: "IX_AppSupportTickets_Category",
                table: "AppSupportTickets",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AppSupportTickets_Status",
                table: "AppSupportTickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppSupportTickets_UserId",
                table: "AppSupportTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTrainerSubscriptions_SubscriptionPlanId",
                table: "AppTrainerSubscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTrainerSubscriptions_TrainerProfileId",
                table: "AppTrainerSubscriptions",
                column: "TrainerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTrainerSubscriptions_TrainerProfileId_Status",
                table: "AppTrainerSubscriptions",
                columns: new[] { "TrainerProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AppTrainerWallets_TrainerProfileId",
                table: "AppTrainerWallets",
                column: "TrainerProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUserProfiles_UserId",
                table: "AppUserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppWalletTransactions_TrainerWalletId",
                table: "AppWalletTransactions",
                column: "TrainerWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWalletTransactions_TransactionType",
                table: "AppWalletTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_AppWithdrawalRequests_Status",
                table: "AppWithdrawalRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppWithdrawalRequests_TrainerWalletId",
                table: "AppWithdrawalRequests",
                column: "TrainerWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppReviews_AppServicePackages_ServicePackageId",
                table: "AppReviews",
                column: "ServicePackageId",
                principalTable: "AppServicePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppSessions_AppSessions_RescheduledFromSessionId",
                table: "AppSessions",
                column: "RescheduledFromSessionId",
                principalTable: "AppSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppReviews_AppServicePackages_ServicePackageId",
                table: "AppReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_AppSessions_AppSessions_RescheduledFromSessionId",
                table: "AppSessions");

            migrationBuilder.DropTable(
                name: "AppBlogPosts");

            migrationBuilder.DropTable(
                name: "AppDisputes");

            migrationBuilder.DropTable(
                name: "AppFeaturedListings");

            migrationBuilder.DropTable(
                name: "AppNotifications");

            migrationBuilder.DropTable(
                name: "AppPayments");

            migrationBuilder.DropTable(
                name: "AppReviewHelpfulVotes");

            migrationBuilder.DropTable(
                name: "AppSupportTickets");

            migrationBuilder.DropTable(
                name: "AppTrainerSubscriptions");

            migrationBuilder.DropTable(
                name: "AppUserProfiles");

            migrationBuilder.DropTable(
                name: "AppWalletTransactions");

            migrationBuilder.DropTable(
                name: "AppWithdrawalRequests");

            migrationBuilder.DropTable(
                name: "AppSubscriptionPlans");

            migrationBuilder.DropTable(
                name: "AppTrainerWallets");

            migrationBuilder.DropIndex(
                name: "IX_AppSessions_RescheduledFromSessionId",
                table: "AppSessions");

            migrationBuilder.DropIndex(
                name: "IX_AppReviews_ServicePackageId",
                table: "AppReviews");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_PaymentId",
                table: "AppOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId_OrderId",
                table: "AppConversations");

            migrationBuilder.DropIndex(
                name: "IX_AppConversations_OrderId",
                table: "AppConversations");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "AppSessions");

            migrationBuilder.DropColumn(
                name: "RescheduledFromSessionId",
                table: "AppSessions");

            migrationBuilder.DropColumn(
                name: "CommunicationRating",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "ExpertiseRating",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "HelpfulCount",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "IsVerifiedPurchase",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "OverallRating",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "PunctualityRating",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "ServicePackageId",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "ValueForMoneyRating",
                table: "AppReviews");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "ProgramAttachmentUrl",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "ProgramDeliveredAt",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "StudentFormData",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "StudentFormSubmittedAt",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TrainerProgramNotes",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "AppConversations");

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId",
                table: "AppConversations",
                columns: new[] { "InitiatorId", "ParticipantId" },
                unique: true);

        }
    }
}
