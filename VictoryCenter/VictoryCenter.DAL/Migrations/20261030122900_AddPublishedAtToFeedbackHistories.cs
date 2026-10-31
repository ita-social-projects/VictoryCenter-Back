using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishedAtToFeedbackHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PublishedAt",
                table: "FeedbackHistories",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackHistories_Status_PublishedAt",
                table: "FeedbackHistories",
                columns: new[] { "Status", "PublishedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeedbackHistories_Status_PublishedAt",
                table: "FeedbackHistories");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "FeedbackHistories");
        }
    }
}
