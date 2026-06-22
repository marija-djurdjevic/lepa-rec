using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerspectiveScenarioAnswerConversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerConversations",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    GuideIterationCount = table.Column<int>(type: "integer", nullable: false),
                    MaxGuideIterations = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerConversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnswerConversationTurns",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EvaluationMark = table.Column<int>(type: "integer", nullable: true),
                    EvaluationLanguage = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EvaluationIssues = table.Column<string>(type: "text", nullable: true),
                    EvaluationStrengths = table.Column<string>(type: "text", nullable: true),
                    WhyThisQuestion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AnswerConversationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerConversationTurns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerConversationTurns_AnswerConversations_AnswerConversationId",
                        column: x => x.AnswerConversationId,
                        principalSchema: "practice",
                        principalTable: "AnswerConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerConversations_UserId_ExerciseId_QuestionId",
                schema: "practice",
                table: "AnswerConversations",
                columns: new[] { "UserId", "ExerciseId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnswerConversationTurns_AnswerConversationId_IdempotencyKey",
                schema: "practice",
                table: "AnswerConversationTurns",
                columns: new[] { "AnswerConversationId", "IdempotencyKey" },
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerConversationTurns",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "AnswerConversations",
                schema: "practice");
        }
    }
}
