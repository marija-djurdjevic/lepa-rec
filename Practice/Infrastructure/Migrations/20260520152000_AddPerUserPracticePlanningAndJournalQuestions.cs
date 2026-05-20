using System;
using AngularNetBase.Practice.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    [DbContext(typeof(PracticeContext))]
    [Migration("20260520152000_AddPerUserPracticePlanningAndJournalQuestions")]
    public partial class AddPerUserPracticePlanningAndJournalQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Theme",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Variant",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "A");

            migrationBuilder.AddColumn<string>(
                name: "Phase",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Single");

            migrationBuilder.Sql(
                """
                UPDATE practice."DistancedJournalChallenges"
                SET "Theme" = LEFT("Content", 300)
                WHERE "Theme" = '';
                """);

            migrationBuilder.CreateTable(
                name: "DistancedJournalQuestions",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DistancedJournalChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    TextEn = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistancedJournalQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistancedJournalQuestions_DistancedJournalChallenges_DistancedJournalChallengeId",
                        column: x => x.DistancedJournalChallengeId,
                        principalSchema: "practice",
                        principalTable: "DistancedJournalChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistancedJournalQuestions_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "practice",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserChallengeExposures",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShownOnDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChallengeExposures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDailyPracticeAssignments",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    MainExerciseType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DistancedJournalChallengeId = table.Column<Guid>(type: "uuid", nullable: true),
                    DistancedJournalChallengeId2 = table.Column<Guid>(type: "uuid", nullable: true),
                    PerspectiveScenarioChallengeId = table.Column<Guid>(type: "uuid", nullable: true),
                    PerspectiveScenarioChallengeId2 = table.Column<Guid>(type: "uuid", nullable: true),
                    ReflectionExerciseId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDailyPracticeAssignments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistancedJournalQuestions_DistancedJournalChallengeId_Kind",
                schema: "practice",
                table: "DistancedJournalQuestions",
                columns: new[] { "DistancedJournalChallengeId", "Kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistancedJournalQuestions_DistancedJournalChallengeId_Order",
                schema: "practice",
                table: "DistancedJournalQuestions",
                columns: new[] { "DistancedJournalChallengeId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistancedJournalQuestions_SkillId",
                schema: "practice",
                table: "DistancedJournalQuestions",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallengeExposures_UserId_Type_ChallengeId",
                schema: "practice",
                table: "UserChallengeExposures",
                columns: new[] { "UserId", "Type", "ChallengeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDailyPracticeAssignments_UserId_Date",
                schema: "practice",
                table: "UserDailyPracticeAssignments",
                columns: new[] { "UserId", "Date" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistancedJournalQuestions",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "UserChallengeExposures",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "UserDailyPracticeAssignments",
                schema: "practice");

            migrationBuilder.DropColumn(
                name: "Theme",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropColumn(
                name: "Variant",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropColumn(
                name: "Phase",
                schema: "practice",
                table: "DistancedJournalChallenges");
        }
    }
}
