using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityAndIsHiddenToMetric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Metrics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "Priority",
                table: "Metrics",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Metrics");
        }
    }
}
