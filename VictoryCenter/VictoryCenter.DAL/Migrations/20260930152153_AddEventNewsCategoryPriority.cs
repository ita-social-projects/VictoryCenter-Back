using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddEventNewsCategoryPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Priority",
                table: "EventNewsEventNewsCategories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql(
            """
                WITH RankedEventNews AS
                (
                    SELECT
                        EventsNewsId,
                        CategoriesId,
                        ROW_NUMBER() OVER (
                            PARTITION BY CategoriesId
                            ORDER BY EventsNewsId DESC
                        ) AS Priority
                    FROM EventNewsEventNewsCategories
                )
                UPDATE target
                SET target.Priority = ranked.Priority
                FROM EventNewsEventNewsCategories AS target
                INNER JOIN RankedEventNews AS ranked
                    ON target.EventsNewsId = ranked.EventsNewsId
                    AND target.CategoriesId = ranked.CategoriesId;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "EventNewsEventNewsCategories");
        }
    }
}
