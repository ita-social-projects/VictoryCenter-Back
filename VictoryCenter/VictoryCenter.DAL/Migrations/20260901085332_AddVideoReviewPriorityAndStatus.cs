using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoReviewPriorityAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Priority",
                table: "VideoReviews",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "VideoReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE vr
                SET vr.Priority = ranked.RowNum
                FROM VideoReviews vr
                INNER JOIN (
                    SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS RowNum
                    FROM VideoReviews
                ) ranked ON vr.Id = ranked.Id;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "VideoReviews");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VideoReviews");
        }
    }
}
