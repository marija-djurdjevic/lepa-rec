using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyChallengeAssignmentSecondOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DistancedJournalChallengeId2",
                schema: "practice",
                table: "DailyChallengeAssignments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PerspectiveScenarioChallengeId2",
                schema: "practice",
                table: "DailyChallengeAssignments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistancedJournalChallengeId2",
                schema: "practice",
                table: "DailyChallengeAssignments");

            migrationBuilder.DropColumn(
                name: "PerspectiveScenarioChallengeId2",
                schema: "practice",
                table: "DailyChallengeAssignments");
        }
    }
}
