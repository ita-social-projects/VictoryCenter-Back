using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameCategoryToTeamCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Categories_CategoryId",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "TeamCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamCategories",
                table: "TeamCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_TeamCategories_CategoryId",
                table: "TeamMembers",
                column: "CategoryId",
                principalTable: "TeamCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_TeamCategories_CategoryId",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamCategories",
                table: "TeamCategories");

            migrationBuilder.RenameTable(
                name: "TeamCategories",
                newName: "Categories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Categories_CategoryId",
                table: "TeamMembers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
