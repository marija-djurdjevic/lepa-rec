using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HookChallengeId",
                schema: "identity",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HookType",
                schema: "identity",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationEnabled",
                schema: "identity",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NotificationTimeLocal",
                schema: "identity",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingCompleted",
                schema: "identity",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OnboardingCompletedAt",
                schema: "identity",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                schema: "identity",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "en");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                schema: "identity",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE identity."AspNetUsers"
                SET "OnboardingCompleted" = TRUE,
                    "OnboardingCompletedAt" = NOW()
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HookChallengeId",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HookType",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NotificationEnabled",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NotificationTimeLocal",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnboardingCompleted",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnboardingCompletedAt",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                schema: "identity",
                table: "AspNetUsers");
        }
    }
}
