using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitliyo.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderReviewMessaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppConversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InitiatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_AppConversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicePackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TrainerPayoutAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PaymentProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaymentTransactionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_AppOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppOrders_AppServicePackages_ServicePackageId",
                        column: x => x.ServicePackageId,
                        principalTable: "AppServicePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppOrders_AppTrainerProfiles_TrainerProfileId",
                        column: x => x.TrainerProfileId,
                        principalTable: "AppTrainerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    AttachmentUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_AppMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMessages_AppConversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "AppConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TrainerReply = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TrainerReplyDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_AppReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppReviews_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppReviews_AppTrainerProfiles_TrainerProfileId",
                        column: x => x.TrainerProfileId,
                        principalTable: "AppTrainerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MeetingUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    TrainerNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StudentNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_AppSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSessions_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_InitiatorId",
                table: "AppConversations",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId",
                table: "AppConversations",
                columns: new[] { "InitiatorId", "ParticipantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_ParticipantId",
                table: "AppConversations",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMessages_ConversationId",
                table: "AppMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMessages_ConversationId_IsRead",
                table: "AppMessages",
                columns: new[] { "ConversationId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_AppMessages_SenderId",
                table: "AppMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_OrderNumber",
                table: "AppOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_ServicePackageId",
                table: "AppOrders",
                column: "ServicePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_Status",
                table: "AppOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_StudentId",
                table: "AppOrders",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_TrainerProfileId",
                table: "AppOrders",
                column: "TrainerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AppReviews_OrderId",
                table: "AppReviews",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppReviews_Rating",
                table: "AppReviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_AppReviews_StudentId",
                table: "AppReviews",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppReviews_TrainerProfileId",
                table: "AppReviews",
                column: "TrainerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSessions_OrderId",
                table: "AppSessions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSessions_ScheduledStartTime",
                table: "AppSessions",
                column: "ScheduledStartTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppSessions_StudentId",
                table: "AppSessions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSessions_TrainerProfileId",
                table: "AppSessions",
                column: "TrainerProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppMessages");

            migrationBuilder.DropTable(
                name: "AppReviews");

            migrationBuilder.DropTable(
                name: "AppSessions");

            migrationBuilder.DropTable(
                name: "AppConversations");

            migrationBuilder.DropTable(
                name: "AppOrders");
        }
    }
}
