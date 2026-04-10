using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMindsetPrimer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrimerAffirmationId",
                schema: "practice",
                table: "DailySessions",
                newName: "PrimerSelectedStatementId");

            migrationBuilder.AddColumn<string>(
                name: "PrimerPresentedStatementIds",
                schema: "practice",
                table: "DailySessions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AffirmationValues",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffirmationValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrowthMessages",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValueStatements",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AffirmationValueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueStatements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValueStatements_AffirmationValues_AffirmationValueId",
                        column: x => x.AffirmationValueId,
                        principalSchema: "practice",
                        principalTable: "AffirmationValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ValueStatements_AffirmationValueId",
                schema: "practice",
                table: "ValueStatements",
                column: "AffirmationValueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrowthMessages",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "ValueStatements",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "AffirmationValues",
                schema: "practice");

            migrationBuilder.DropColumn(
                name: "PrimerPresentedStatementIds",
                schema: "practice",
                table: "DailySessions");

            migrationBuilder.RenameColumn(
                name: "PrimerSelectedStatementId",
                schema: "practice",
                table: "DailySessions",
                newName: "PrimerAffirmationId");
        }
    }
}
