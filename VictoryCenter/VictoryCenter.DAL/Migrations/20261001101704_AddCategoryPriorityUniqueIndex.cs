using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryPriorityUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EventNewsEventNewsCategories_CategoriesId_Priority",
                table: "EventNewsEventNewsCategories",
                columns: new[] { "CategoriesId", "Priority" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventNewsEventNewsCategories_CategoriesId_Priority",
                table: "EventNewsEventNewsCategories");
        }
    }
}
