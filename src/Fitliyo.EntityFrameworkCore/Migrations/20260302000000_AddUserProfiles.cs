using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitliyo.Migrations
{
    /// <inheritdoc />
    [Migration("20260302000000_AddUserProfiles")]
    public partial class AddUserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    HeightCm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    WeightKg = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
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
                    WaistCm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    HipCm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    NeckCm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TargetWeightKg = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_AppUserProfiles_UserId",
                table: "AppUserProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserProfiles");
        }
    }
}
