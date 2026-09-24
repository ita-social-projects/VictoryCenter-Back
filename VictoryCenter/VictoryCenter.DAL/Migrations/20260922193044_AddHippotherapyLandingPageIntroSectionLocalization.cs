using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHippotherapyLandingPageIntroSectionLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageIntroSectionLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageIntroSectionLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageIntroSectionLocalizations_HippotherapyLandingPageIntroSections_EntityId",
                        column: x => x.EntityId,
                        principalTable: "HippotherapyLandingPageIntroSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageIntroSectionLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageIntroSectionLocalizations_LanguageId",
                table: "HippotherapyLandingPageIntroSectionLocalizations",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageIntroSectionLocalizations");
        }
    }
}
