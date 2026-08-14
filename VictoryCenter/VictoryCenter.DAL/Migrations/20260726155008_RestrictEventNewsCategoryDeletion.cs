using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RestrictEventNewsCategoryDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventNewsEventNewsCategories_EventNewsCategories_CategoriesId",
                table: "EventNewsEventNewsCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_EventNewsEventNewsCategories_EventNewsCategories_CategoriesId",
                table: "EventNewsEventNewsCategories",
                column: "CategoriesId",
                principalTable: "EventNewsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventNewsEventNewsCategories_EventNewsCategories_CategoriesId",
                table: "EventNewsEventNewsCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_EventNewsEventNewsCategories_EventNewsCategories_CategoriesId",
                table: "EventNewsEventNewsCategories",
                column: "CategoriesId",
                principalTable: "EventNewsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
