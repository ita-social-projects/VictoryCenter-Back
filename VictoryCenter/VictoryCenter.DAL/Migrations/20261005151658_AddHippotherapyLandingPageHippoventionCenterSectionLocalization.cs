using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHippotherapyLandingPageHippoventionCenterSectionLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageHippoventionCenterSectionLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pros = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageHippoventionCenterSectionLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageHippoventionCenterSectionLocalizations_HippotherapyLandingPageHippoventionCenterSections_EntityId",
                        column: x => x.EntityId,
                        principalTable: "HippotherapyLandingPageHippoventionCenterSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageHippoventionCenterSectionLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageHippoventionCenterSectionLocalizations_LanguageId",
                table: "HippotherapyLandingPageHippoventionCenterSectionLocalizations",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageHippoventionCenterSectionLocalizations");
        }
    }
}
