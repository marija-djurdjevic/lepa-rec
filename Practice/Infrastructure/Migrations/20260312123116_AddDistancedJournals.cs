using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDistancedJournals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DistancedJournalChallenges",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    FollowUpQuestion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ChallengeLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistancedJournalChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DistancedJournalExercises",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MainAnswer = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    FollowUpAnswer = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Reflection = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistancedJournalExercises", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistancedJournalExercises_UserId_ChallengeId",
                schema: "practice",
                table: "DistancedJournalExercises",
                columns: new[] { "UserId", "ChallengeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistancedJournalChallenges",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "DistancedJournalExercises",
                schema: "practice");
        }
    }
}
