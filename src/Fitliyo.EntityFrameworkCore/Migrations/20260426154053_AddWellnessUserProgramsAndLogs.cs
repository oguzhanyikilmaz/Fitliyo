using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitliyo.Migrations
{
    /// <inheritdoc />
    public partial class AddWellnessUserProgramsAndLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPersonalNutritionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DailyCalorieTarget = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    DailyProteinTargetG = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    DailyCarbsTargetG = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    DailyFatTargetG = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
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
                    table.PrimaryKey("PK_AppPersonalNutritionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPersonalWorkoutPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    WeekdayIndex = table.Column<int>(type: "integer", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_AppPersonalWorkoutPrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserDailyMealLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_AppUserDailyMealLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserFoodItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    KcalPer100G = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    ProteinPer100G = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    CarbsPer100G = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    FatPer100G = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_AppUserFoodItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserNotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailOrdersAndSessions = table.Column<bool>(type: "boolean", nullable: false),
                    EmailMarketing = table.Column<bool>(type: "boolean", nullable: false),
                    PushChat = table.Column<bool>(type: "boolean", nullable: false),
                    PushOrderSession = table.Column<bool>(type: "boolean", nullable: false),
                    PushWellnessReminders = table.Column<bool>(type: "boolean", nullable: false),
                    InAppAll = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_AppUserNotificationPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPersonalWorkoutTemplateExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonalWorkoutProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayNumber = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TargetSets = table.Column<int>(type: "integer", nullable: true),
                    TargetReps = table.Column<int>(type: "integer", nullable: true),
                    SuggestedDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    DefaultMet = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPersonalWorkoutTemplateExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPersonalWorkoutTemplateExercises_AppPersonalWorkoutProgr~",
                        column: x => x.PersonalWorkoutProgramId,
                        principalTable: "AppPersonalWorkoutPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserWorkoutLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonalWorkoutProgramId = table.Column<Guid>(type: "uuid", nullable: true),
                    LogDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TotalCaloriesBurned = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_AppUserWorkoutLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserWorkoutLogs_AppPersonalWorkoutPrograms_PersonalWorko~",
                        column: x => x.PersonalWorkoutProgramId,
                        principalTable: "AppPersonalWorkoutPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AppUserMealLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserDailyMealLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    MealType = table.Column<int>(type: "integer", nullable: false),
                    UserFoodItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    FoodName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PortionGrams = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Calories = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    ProteinG = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    CarbsG = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    FatG = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserMealLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserMealLogEntries_AppUserDailyMealLogs_UserDailyMealLog~",
                        column: x => x.UserDailyMealLogId,
                        principalTable: "AppUserDailyMealLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserMealLogEntries_AppUserFoodItems_UserFoodItemId",
                        column: x => x.UserFoodItemId,
                        principalTable: "AppUserFoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AppUserWorkoutLogLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserWorkoutLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DurationMinutes = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Met = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    CaloriesBurned = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserWorkoutLogLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserWorkoutLogLines_AppUserWorkoutLogs_UserWorkoutLogId",
                        column: x => x.UserWorkoutLogId,
                        principalTable: "AppUserWorkoutLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPersonalNutritionPlans_IsActive",
                table: "AppPersonalNutritionPlans",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AppPersonalNutritionPlans_UserId",
                table: "AppPersonalNutritionPlans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPersonalWorkoutPrograms_IsArchived",
                table: "AppPersonalWorkoutPrograms",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_AppPersonalWorkoutPrograms_UserId",
                table: "AppPersonalWorkoutPrograms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPersonalWorkoutTemplateExercises_PersonalWorkoutProgramId",
                table: "AppPersonalWorkoutTemplateExercises",
                column: "PersonalWorkoutProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserDailyMealLogs_UserId_LogDate",
                table: "AppUserDailyMealLogs",
                columns: new[] { "UserId", "LogDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUserFoodItems_UserId",
                table: "AppUserFoodItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserMealLogEntries_UserDailyMealLogId",
                table: "AppUserMealLogEntries",
                column: "UserDailyMealLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserMealLogEntries_UserFoodItemId",
                table: "AppUserMealLogEntries",
                column: "UserFoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserNotificationPreferences_UserId",
                table: "AppUserNotificationPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUserWorkoutLogLines_UserWorkoutLogId",
                table: "AppUserWorkoutLogLines",
                column: "UserWorkoutLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserWorkoutLogs_PersonalWorkoutProgramId",
                table: "AppUserWorkoutLogs",
                column: "PersonalWorkoutProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserWorkoutLogs_UserId_LogDate",
                table: "AppUserWorkoutLogs",
                columns: new[] { "UserId", "LogDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPersonalNutritionPlans");

            migrationBuilder.DropTable(
                name: "AppPersonalWorkoutTemplateExercises");

            migrationBuilder.DropTable(
                name: "AppUserMealLogEntries");

            migrationBuilder.DropTable(
                name: "AppUserNotificationPreferences");

            migrationBuilder.DropTable(
                name: "AppUserWorkoutLogLines");

            migrationBuilder.DropTable(
                name: "AppUserDailyMealLogs");

            migrationBuilder.DropTable(
                name: "AppUserFoodItems");

            migrationBuilder.DropTable(
                name: "AppUserWorkoutLogs");

            migrationBuilder.DropTable(
                name: "AppPersonalWorkoutPrograms");
        }
    }
}
