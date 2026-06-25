using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardPuzzleProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RewardImages",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AssetPath = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRewardProgresses",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RewardImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnlockedPiecesCount = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SavedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRewardProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRewardProgresses_RewardImages_RewardImageId",
                        column: x => x.RewardImageId,
                        principalSchema: "practice",
                        principalTable: "RewardImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "practice",
                table: "RewardImages",
                columns: new[] { "Id", "AssetKey", "AssetPath", "ImageUrl", "SortOrder", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000001"), "reward_01", "assets/images/rewards/reward_01.png", null, 1, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000002"), "reward_02", "assets/images/rewards/reward_02.png", null, 2, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000003"), "reward_03", "assets/images/rewards/reward_03.png", null, 3, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000004"), "reward_04", "assets/images/rewards/reward_04.png", null, 4, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000005"), "reward_05", "assets/images/rewards/reward_05.png", null, 5, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000006"), "reward_06", "assets/images/rewards/reward_06.png", null, 6, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000007"), "reward_07", "assets/images/rewards/reward_07.png", null, 7, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000008"), "reward_08", "assets/images/rewards/reward_08.png", null, 8, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000009"), "reward_09", "assets/images/rewards/reward_09.png", null, 9, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6b8c8f0d-1e2a-4f3b-9c5d-000000000010"), "reward_10", "assets/images/rewards/reward_10.png", null, 10, true, new DateTime(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateTable(
                name: "RewardPieceGrants",
                schema: "practice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DailySessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionDate = table.Column<DateTime>(type: "date", nullable: false),
                    RewardProgressId = table.Column<Guid>(type: "uuid", nullable: false),
                    PieceIndex = table.Column<int>(type: "integer", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardPieceGrants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RewardPieceGrants_UserRewardProgresses_RewardProgressId",
                        column: x => x.RewardProgressId,
                        principalSchema: "practice",
                        principalTable: "UserRewardProgresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RewardImages_AssetKey",
                schema: "practice",
                table: "RewardImages",
                column: "AssetKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardPieceGrants_RewardProgressId",
                schema: "practice",
                table: "RewardPieceGrants",
                column: "RewardProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardPieceGrants_UserId_DailySessionId",
                schema: "practice",
                table: "RewardPieceGrants",
                columns: new[] { "UserId", "DailySessionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardPieceGrants_UserId_SessionDate",
                schema: "practice",
                table: "RewardPieceGrants",
                columns: new[] { "UserId", "SessionDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRewardProgresses_RewardImageId",
                schema: "practice",
                table: "UserRewardProgresses",
                column: "RewardImageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRewardProgresses_UserId",
                schema: "practice",
                table: "UserRewardProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRewardProgresses_UserId_CompletedAt",
                schema: "practice",
                table: "UserRewardProgresses",
                columns: new[] { "UserId", "CompletedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardPieceGrants",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "UserRewardProgresses",
                schema: "practice");

            migrationBuilder.DropTable(
                name: "RewardImages",
                schema: "practice");
        }
    }
}
