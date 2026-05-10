using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingHookMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnboardingHookRun",
                schema: "practice",
                table: "PerspectiveScenarioExercises",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnboardingHook",
                schema: "practice",
                table: "PerspectiveScenarioChallenges",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OnboardingHookKey",
                schema: "practice",
                table: "PerspectiveScenarioChallenges",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnboardingHookRun",
                schema: "practice",
                table: "DistancedJournalExercises",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnboardingHook",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OnboardingHookKey",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerspectiveScenarioChallenges_OnboardingHookKey",
                schema: "practice",
                table: "PerspectiveScenarioChallenges",
                column: "OnboardingHookKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistancedJournalChallenges_OnboardingHookKey",
                schema: "practice",
                table: "DistancedJournalChallenges",
                column: "OnboardingHookKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PerspectiveScenarioChallenges_OnboardingHookKey",
                schema: "practice",
                table: "PerspectiveScenarioChallenges");

            migrationBuilder.DropIndex(
                name: "IX_DistancedJournalChallenges_OnboardingHookKey",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropColumn(
                name: "IsOnboardingHookRun",
                schema: "practice",
                table: "PerspectiveScenarioExercises");

            migrationBuilder.DropColumn(
                name: "IsOnboardingHook",
                schema: "practice",
                table: "PerspectiveScenarioChallenges");

            migrationBuilder.DropColumn(
                name: "OnboardingHookKey",
                schema: "practice",
                table: "PerspectiveScenarioChallenges");

            migrationBuilder.DropColumn(
                name: "IsOnboardingHookRun",
                schema: "practice",
                table: "DistancedJournalExercises");

            migrationBuilder.DropColumn(
                name: "IsOnboardingHook",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropColumn(
                name: "OnboardingHookKey",
                schema: "practice",
                table: "DistancedJournalChallenges");
        }
    }
}
