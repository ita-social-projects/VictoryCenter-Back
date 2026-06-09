using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageIdToPdfReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LanguageId",
                schema: "media",
                table: "PdfReports",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PdfReports_LanguageId",
                schema: "media",
                table: "PdfReports",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PdfReports_LocalizationLanguages_LanguageId",
                schema: "media",
                table: "PdfReports",
                column: "LanguageId",
                principalTable: "LocalizationLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PdfReports_LocalizationLanguages_LanguageId",
                schema: "media",
                table: "PdfReports");

            migrationBuilder.DropIndex(
                name: "IX_PdfReports_LanguageId",
                schema: "media",
                table: "PdfReports");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                schema: "media",
                table: "PdfReports");
        }
    }
}
