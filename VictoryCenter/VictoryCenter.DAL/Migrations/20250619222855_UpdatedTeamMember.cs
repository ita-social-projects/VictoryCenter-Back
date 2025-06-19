using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTeamMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Categories_CategoryId",
                table: "TeamMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Categories_CategoryId",
                table: "TeamMembers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Categories_CategoryId",
                table: "TeamMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Categories_CategoryId",
                table: "TeamMembers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
