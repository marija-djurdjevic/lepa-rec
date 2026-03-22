using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerspectiveScenariosAndSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerspectiveScenarioChallenges",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChallengeLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ScenarioText = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Reveal = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerspectiveScenarioChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerspectiveScenarioExercises",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerspectiveScenarioExercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillMasteries",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentLevel = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillMasteries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerspectiveScenarioQuestions",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PerspectiveScenarioChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerspectiveScenarioQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerspectiveScenarioQuestions_PerspectiveScenarioChallenges_~",
                        column: x => x.PerspectiveScenarioChallengeId,
                        principalSchema: "practice",
                        principalTable: "PerspectiveScenarioChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerspectiveScenarioAnswers",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerText = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    PerspectiveScenarioExerciseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerspectiveScenarioAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerspectiveScenarioAnswers_PerspectiveScenarioExercises_Per~",
                        column: x => x.PerspectiveScenarioExerciseId,
                        principalSchema: "practice",
                        principalTable: "PerspectiveScenarioExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillLevelDefinitions",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelNumber = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillLevelDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillLevelDefinitions_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "practice",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerspectiveScenarioAnswers_PerspectiveScenarioExerciseId",
                schema: "practice",
                table: "PerspectiveScenarioAnswers",
                column: "PerspectiveScenarioExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_PerspectiveScenarioExercises_UserId_ChallengeId",
                schema: "practice",
                table: "PerspectiveScenarioExercises",
                columns: new[] { "UserId", "ChallengeId" });

            migrationBuilder.CreateIndex(
                name: "IX_PerspectiveScenarioQuestions_PerspectiveScenarioChallengeId",
                schema: "practice",
                table: "PerspectiveScenarioQuestions",
                column: "PerspectiveScenarioChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevelDefinitions_SkillId",
                schema: "practice",
                table: "SkillLevelDefinitions",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillMasteries_UserId_SkillId",
                schema: "practice",
                table: "SkillMasteries",
                columns: new[] { "UserId", "SkillId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerspectiveScenarioAnswers",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "PerspectiveScenarioQuestions",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "SkillLevelDefinitions",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "SkillMasteries",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "PerspectiveScenarioExercises",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "PerspectiveScenarioChallenges",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "Skills",
                schema: "practice");
        }
    }
}
