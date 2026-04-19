using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPracticeContentSchemaUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PerspectiveScenarioQuestions_PerspectiveScenarioChallengeId",
                schema: "practice",
                table: "PerspectiveScenarioQuestions");

            migrationBuilder.DropColumn(
                name: "Reveal",
                schema: "practice",
                table: "PerspectiveScenarioChallenges");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "practice",
                table: "PerspectiveScenarioQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Reveal",
                schema: "practice",
                table: "PerspectiveScenarioQuestions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "AffirmationValueId",
                schema: "practice",
                table: "GrowthMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SkillId",
                schema: "practice",
                table: "DistancedJournalChallenges",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerspectiveScenarioQuestions_PerspectiveScenarioChallengeId~",
                schema: "practice",
                table: "PerspectiveScenarioQuestions",
                columns: new[] { "PerspectiveScenarioChallengeId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrowthMessages_AffirmationValueId",
                schema: "practice",
                table: "GrowthMessages",
                column: "AffirmationValueId");

            migrationBuilder.CreateIndex(
                name: "IX_DistancedJournalChallenges_SkillId",
                schema: "practice",
                table: "DistancedJournalChallenges",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_DistancedJournalChallenges_Skills_SkillId",
                schema: "practice",
                table: "DistancedJournalChallenges",
                column: "SkillId",
                principalSchema: "practice",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_GrowthMessages_AffirmationValues_AffirmationValueId",
                schema: "practice",
                table: "GrowthMessages",
                column: "AffirmationValueId",
                principalSchema: "practice",
                principalTable: "AffirmationValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistancedJournalChallenges_Skills_SkillId",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropForeignKey(
                name: "FK_GrowthMessages_AffirmationValues_AffirmationValueId",
                schema: "practice",
                table: "GrowthMessages");

            migrationBuilder.DropIndex(
                name: "IX_PerspectiveScenarioQuestions_PerspectiveScenarioChallengeId~",
                schema: "practice",
                table: "PerspectiveScenarioQuestions");

            migrationBuilder.DropIndex(
                name: "IX_GrowthMessages_AffirmationValueId",
                schema: "practice",
                table: "GrowthMessages");

            migrationBuilder.DropIndex(
                name: "IX_DistancedJournalChallenges_SkillId",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "practice",
                table: "PerspectiveScenarioQuestions");

            migrationBuilder.DropColumn(
                name: "Reveal",
                schema: "practice",
                table: "PerspectiveScenarioQuestions");

            migrationBuilder.DropColumn(
                name: "AffirmationValueId",
                schema: "practice",
                table: "GrowthMessages");

            migrationBuilder.DropColumn(
                name: "SkillId",
                schema: "practice",
                table: "DistancedJournalChallenges");

            migrationBuilder.AddColumn<string>(
                name: "Reveal",
                schema: "practice",
                table: "PerspectiveScenarioChallenges",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PerspectiveScenarioQuestions_PerspectiveScenarioChallengeId",
                schema: "practice",
                table: "PerspectiveScenarioQuestions",
                column: "PerspectiveScenarioChallengeId");
        }
    }
}
