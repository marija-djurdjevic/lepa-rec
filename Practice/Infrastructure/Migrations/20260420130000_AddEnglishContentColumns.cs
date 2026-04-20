using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    [Migration("20260420130000_AddEnglishContentColumns")]
    public partial class AddEnglishContentColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpQuestionEn",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                schema: "practice",
                table: "GrowthMessages",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScenarioTextEn",
                schema: "practice",
                table: "PerspectiveScenarioChallenges",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionTextEn",
                schema: "practice",
                table: "PerspectiveScenarioQuestions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevealEn",
                schema: "practice",
                table: "PerspectiveScenarioQuestions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentEn",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropColumn(
                name: "FollowUpQuestionEn",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropColumn(
                name: "TextEn",
                schema: "practice",
                table: "GrowthMessages");

            migrationBuilder.DropColumn(
                name: "ScenarioTextEn",
                schema: "practice",
                table: "PerspectiveScenarioChallenges");

            migrationBuilder.DropColumn(
                name: "QuestionTextEn",
                schema: "practice",
                table: "PerspectiveScenarioQuestions");

            migrationBuilder.DropColumn(
                name: "RevealEn",
                schema: "practice",
                table: "PerspectiveScenarioQuestions");
        }
    }
}
