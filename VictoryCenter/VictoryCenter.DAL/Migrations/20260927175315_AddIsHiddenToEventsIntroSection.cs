using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHiddenToEventsIntroSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEventsBlockTitleHidden",
                table: "EventsIntroSections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPageDescriptionHidden",
                table: "EventsIntroSections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEventsBlockTitleHidden",
                table: "EventsIntroSections");

            migrationBuilder.DropColumn(
                name: "IsPageDescriptionHidden",
                table: "EventsIntroSections");
        }
    }
}
