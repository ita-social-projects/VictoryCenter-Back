using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEventNewsCategoryRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "EventNews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Priority",
                table: "EventNews",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql(
                """
                DECLARE @FallbackCategoryId bigint;

                IF EXISTS
                (
                    SELECT 1
                    FROM EventNews AS eventNews
                    WHERE NOT EXISTS
                    (
                        SELECT 1
                        FROM EventNewsEventNewsCategories AS relation
                        WHERE relation.EventsNewsId = eventNews.Id
                    )
                )
                BEGIN
                    SELECT TOP (1)
                        @FallbackCategoryId = Id
                    FROM EventNewsCategories
                    WHERE Name = N'Інше'
                    ORDER BY Id;

                    IF @FallbackCategoryId IS NULL
                    BEGIN
                        INSERT INTO EventNewsCategories (Name, CreatedAt)
                        VALUES (N'Інше', SYSDATETIMEOFFSET());

                        SET @FallbackCategoryId = SCOPE_IDENTITY();
                    END;
                END;

                ;WITH RankedEventCategories AS
                (
                    SELECT
                        EventsNewsId,
                        CategoriesId,
                        Priority,
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY EventsNewsId
                            ORDER BY Priority, CategoriesId
                        ) AS CategoryRank
                    FROM EventNewsEventNewsCategories
                ),
                SelectedEventCategories AS
                (
                    SELECT
                        EventsNewsId,
                        CategoriesId,
                        Priority
                    FROM RankedEventCategories
                    WHERE CategoryRank = 1

                    UNION ALL

                    SELECT
                        eventNews.Id,
                        @FallbackCategoryId,
                        eventNews.Id
                    FROM EventNews AS eventNews
                    WHERE NOT EXISTS
                    (
                        SELECT 1
                        FROM EventNewsEventNewsCategories AS relation
                        WHERE relation.EventsNewsId = eventNews.Id
                    )
                ),
                RecalculatedPriorities AS
                (
                    SELECT
                        EventsNewsId,
                        CategoriesId,
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY CategoriesId
                            ORDER BY Priority, EventsNewsId
                        ) AS NewPriority
                    FROM SelectedEventCategories
                )
                UPDATE eventNews
                SET
                    eventNews.CategoryId = selected.CategoriesId,
                    eventNews.Priority = selected.NewPriority
                FROM EventNews AS eventNews
                INNER JOIN RecalculatedPriorities AS selected
                    ON selected.EventsNewsId = eventNews.Id;
                """);

            migrationBuilder.AlterColumn<long>(
                name: "CategoryId",
                table: "EventNews",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Priority",
                table: "EventNews",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventNews_CategoryId_Priority",
                table: "EventNews",
                columns: new[] { "CategoryId", "Priority" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventNews_EventNewsCategories_CategoryId",
                table: "EventNews",
                column: "CategoryId",
                principalTable: "EventNewsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropTable(
                name: "EventNewsEventNewsCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventNews_EventNewsCategories_CategoryId",
                table: "EventNews");

            migrationBuilder.DropIndex(
                name: "IX_EventNews_CategoryId_Priority",
                table: "EventNews");

            migrationBuilder.CreateTable(
                name: "EventNewsEventNewsCategories",
                columns: table => new
                {
                    CategoriesId = table.Column<long>(type: "bigint", nullable: false),
                    EventsNewsId = table.Column<long>(type: "bigint", nullable: false),
                    Priority = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNewsEventNewsCategories", x => new { x.CategoriesId, x.EventsNewsId });
                    table.ForeignKey(
                        name: "FK_EventNewsEventNewsCategories_EventNewsCategories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "EventNewsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventNewsEventNewsCategories_EventNews_EventsNewsId",
                        column: x => x.EventsNewsId,
                        principalTable: "EventNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventNewsEventNewsCategories_CategoriesId_Priority",
                table: "EventNewsEventNewsCategories",
                columns: new[] { "CategoriesId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventNewsEventNewsCategories_EventsNewsId",
                table: "EventNewsEventNewsCategories",
                column: "EventsNewsId");

            migrationBuilder.Sql(
                """
                INSERT INTO EventNewsEventNewsCategories
                    (CategoriesId, EventsNewsId, Priority)
                SELECT
                    CategoryId,
                    Id,
                    Priority
                FROM EventNews;
                """);

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "EventNews");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "EventNews");
        }
    }
}
