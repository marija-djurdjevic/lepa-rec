using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    public partial class RemovePracticeVersionColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "AffirmationValues");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "ValueStatements");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "DistancedJournalExercises");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "GrowthMessages");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "PerspectiveScenarioChallenges");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "PerspectiveScenarioExercises");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "PerspectiveScenarioQuestions");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "DailySessions");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "practice",
                table: "SkillMasteries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "AffirmationValues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "ValueStatements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "DistancedJournalExercises",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "GrowthMessages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "PerspectiveScenarioChallenges",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "PerspectiveScenarioExercises",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "PerspectiveScenarioQuestions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "DailySessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "Skills",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "practice",
                table: "SkillMasteries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
