using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceHippoventionProsWithSingleField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageHippoventionPros");

            migrationBuilder.AddColumn<string>(
                name: "Pros",
                table: "HippotherapyLandingPageHippoventionCenterSections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pros",
                table: "HippotherapyLandingPageHippoventionCenterSections");

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageHippoventionPros",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippoventionCenterSectionId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageHippoventionPros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageHippoventionPros_HippotherapyLandingPageHippoventionCenterSections_HippoventionCenterSectionId",
                        column: x => x.HippoventionCenterSectionId,
                        principalTable: "HippotherapyLandingPageHippoventionCenterSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageHippoventionPros_HippoventionCenterSectionId_Priority",
                table: "HippotherapyLandingPageHippoventionPros",
                columns: new[] { "HippoventionCenterSectionId", "Priority" },
                unique: true);
        }
    }
}
