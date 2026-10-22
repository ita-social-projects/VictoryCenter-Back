using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AllowPartialEventsIntroSectionLocalizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PageDescription",
                table: "EventsIntroSectionLocalizations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EventsBlockTitle",
                table: "EventsIntroSectionLocalizations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [EventsIntroSectionLocalizations]
                SET [PageDescription] = N''
                WHERE [PageDescription] IS NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE [EventsIntroSectionLocalizations]
                SET [EventsBlockTitle] = N''
                WHERE [EventsBlockTitle] IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "PageDescription",
                table: "EventsIntroSectionLocalizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventsBlockTitle",
                table: "EventsIntroSectionLocalizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
