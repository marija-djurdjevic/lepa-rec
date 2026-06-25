using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAnswerConversationXminConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PostgreSQL xmin is a system column. This migration only updates the
            // EF model snapshot so AnswerConversation no longer uses it for concurrency.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No database change to reverse; xmin remains a PostgreSQL system column.
        }
    }
}
