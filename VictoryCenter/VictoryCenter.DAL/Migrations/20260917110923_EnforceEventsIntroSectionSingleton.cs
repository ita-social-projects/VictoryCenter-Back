using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EnforceEventsIntroSectionSingleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SingletonKey",
                table: "EventsIntroSections",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_EventsIntroSections_SingletonKey",
                table: "EventsIntroSections",
                column: "SingletonKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventsIntroSections_SingletonKey",
                table: "EventsIntroSections");

            migrationBuilder.DropColumn(
                name: "SingletonKey",
                table: "EventsIntroSections");
        }
    }
}
