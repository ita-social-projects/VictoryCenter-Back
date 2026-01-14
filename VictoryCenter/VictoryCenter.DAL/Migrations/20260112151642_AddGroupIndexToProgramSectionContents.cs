using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupIndexToProgramSectionContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupIndex",
                table: "ProgramSectionContents",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupIndex",
                table: "ProgramSectionContents");
        }
    }
}
