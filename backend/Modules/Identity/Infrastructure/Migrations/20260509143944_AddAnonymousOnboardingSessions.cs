using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonymousOnboardingSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnboardingSessions",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeviceFingerprint = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PreferredLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    HookType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HookChallengeId = table.Column<Guid>(type: "uuid", nullable: true),
                    HookExerciseCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    DistancedExerciseId = table.Column<Guid>(type: "uuid", nullable: true),
                    DistancedSessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DistancedMainAnswer = table.Column<string>(type: "text", nullable: true),
                    DistancedFollowUpAnswer = table.Column<string>(type: "text", nullable: true),
                    DistancedReflection = table.Column<string>(type: "text", nullable: true),
                    PerspectiveExerciseId = table.Column<Guid>(type: "uuid", nullable: true),
                    PerspectiveAnswersJson = table.Column<string>(type: "text", nullable: true),
                    PerspectiveLang = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingSessions_ExpiresAt",
                schema: "identity",
                table: "OnboardingSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingSessions_UsedAt",
                schema: "identity",
                table: "OnboardingSessions",
                column: "UsedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnboardingSessions",
                schema: "identity");
        }
    }
}
