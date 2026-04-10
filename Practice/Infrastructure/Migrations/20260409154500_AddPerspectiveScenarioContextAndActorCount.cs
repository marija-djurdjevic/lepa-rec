using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerspectiveScenarioContextAndActorCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActorCount",
                schema: "practice",
                table: "PerspectiveScenarioChallenges",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Context",
                schema: "practice",
                table: "PerspectiveScenarioChallenges",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Unknown");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActorCount",
                schema: "practice",
                table: "PerspectiveScenarioChallenges");

            migrationBuilder.DropColumn(
                name: "Context",
                schema: "practice",
                table: "PerspectiveScenarioChallenges");
        }
    }
}
