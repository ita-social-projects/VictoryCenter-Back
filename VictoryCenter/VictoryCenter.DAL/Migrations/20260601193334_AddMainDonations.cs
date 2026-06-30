using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMainDonations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MainDonations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    MainPageId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainDonations_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MainDonations_MainPages_MainPageId",
                        column: x => x.MainPageId,
                        principalTable: "MainPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MainDonationsLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainDonationsLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_MainDonationsLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MainDonationsLocalizations_MainDonations_EntityId",
                        column: x => x.EntityId,
                        principalTable: "MainDonations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainDonations_ImageId",
                table: "MainDonations",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MainDonations_MainPageId",
                table: "MainDonations",
                column: "MainPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MainDonationsLocalizations_LanguageId",
                table: "MainDonationsLocalizations",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainDonationsLocalizations");

            migrationBuilder.DropTable(
                name: "MainDonations");
        }
    }
}
