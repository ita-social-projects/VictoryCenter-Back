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
            migrationBuilder.DropTable(
                name: "EventNewsEventNewsCategories");

            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "EventNews",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Priority",
                table: "EventNews",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "EventNews");

            migrationBuilder.DropColumn(
                name: "Priority",
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
        }
    }
}
