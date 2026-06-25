using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneratedDistancedJournalReflection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeneratedReflectionAnswer",
                schema: "practice",
                table: "DistancedJournalExercises",
                type: "character varying(3000)",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneratedReflectionQuestion",
                schema: "practice",
                table: "DistancedJournalExercises",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedReflectionAnswer",
                schema: "practice",
                table: "DistancedJournalExercises");

            migrationBuilder.DropColumn(
                name: "GeneratedReflectionQuestion",
                schema: "practice",
                table: "DistancedJournalExercises");
        }
    }
}
