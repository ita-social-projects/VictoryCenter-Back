using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addImageLogic4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_ImageId",
                table: "TeamMembers");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ImageId",
                table: "TeamMembers",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_ImageId",
                table: "TeamMembers");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ImageId",
                table: "TeamMembers",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");
        }
    }
}
