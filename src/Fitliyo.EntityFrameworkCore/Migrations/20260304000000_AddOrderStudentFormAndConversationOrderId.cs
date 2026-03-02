using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitliyo.Migrations
{
    /// <inheritdoc />
    [Migration("20260304000000_AddOrderStudentFormAndConversationOrderId")]
    public partial class AddOrderStudentFormAndConversationOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<DateTime>(
                name: "ProgramDeliveredAt",
                table: "AppOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgramAttachmentUrl",
                table: "AppOrders",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "AppConversations",
                type: "uuid",
                nullable: true);

            migrationBuilder.DropIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId",
                table: "AppConversations");

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_OrderId",
                table: "AppConversations",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId_OrderId",
                table: "AppConversations",
                columns: new[] { "InitiatorId", "ParticipantId", "OrderId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId_OrderId",
                table: "AppConversations");

            migrationBuilder.DropIndex(
                name: "IX_AppConversations_OrderId",
                table: "AppConversations");

            migrationBuilder.CreateIndex(
                name: "IX_AppConversations_InitiatorId_ParticipantId",
                table: "AppConversations",
                columns: new[] { "InitiatorId", "ParticipantId" },
                unique: true);

            migrationBuilder.DropColumn(name: "StudentFormData", table: "AppOrders");
            migrationBuilder.DropColumn(name: "StudentFormSubmittedAt", table: "AppOrders");
            migrationBuilder.DropColumn(name: "TrainerProgramNotes", table: "AppOrders");
            migrationBuilder.DropColumn(name: "ProgramDeliveredAt", table: "AppOrders");
            migrationBuilder.DropColumn(name: "ProgramAttachmentUrl", table: "AppOrders");
            migrationBuilder.DropColumn(name: "OrderId", table: "AppConversations");
        }
    }
}
