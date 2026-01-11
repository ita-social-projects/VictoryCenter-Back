using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedSlugToHippotherapyPrograms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "HippotherapyPrograms",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyPrograms_Slug",
                table: "HippotherapyPrograms",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HippotherapyPrograms_Slug",
                table: "HippotherapyPrograms");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "HippotherapyPrograms");
        }
    }
}
