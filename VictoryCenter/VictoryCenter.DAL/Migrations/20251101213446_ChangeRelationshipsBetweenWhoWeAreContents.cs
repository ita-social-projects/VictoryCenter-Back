using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationshipsBetweenWhoWeAreContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WhoWeAreContents_CardContent_ImageId",
                table: "WhoWeAreContents");

            migrationBuilder.DropIndex(
                name: "IX_WhoWeAreContents_ImageId",
                table: "WhoWeAreContents");

            migrationBuilder.CreateIndex(
                name: "IX_WhoWeAreContents_CardContent_ImageId",
                table: "WhoWeAreContents",
                column: "CardContent_ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_WhoWeAreContents_ImageId",
                table: "WhoWeAreContents",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WhoWeAreContents_CardContent_ImageId",
                table: "WhoWeAreContents");

            migrationBuilder.DropIndex(
                name: "IX_WhoWeAreContents_ImageId",
                table: "WhoWeAreContents");

            migrationBuilder.CreateIndex(
                name: "IX_WhoWeAreContents_CardContent_ImageId",
                table: "WhoWeAreContents",
                column: "CardContent_ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WhoWeAreContents_ImageId",
                table: "WhoWeAreContents",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");
        }
    }
}
