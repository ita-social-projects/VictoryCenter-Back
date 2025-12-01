using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LocalizationFKFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMemberLocalizations_LocalizationLanguages_LanguageId",
                table: "TeamMemberLocalizations");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "TeamMemberLocalizations",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "LocalizationLanguages",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMemberLocalizations_LocalizationLanguages_LanguageId",
                table: "TeamMemberLocalizations",
                column: "LanguageId",
                principalTable: "LocalizationLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMemberLocalizations_LocalizationLanguages_LanguageId",
                table: "TeamMemberLocalizations");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "TeamMemberLocalizations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "LocalizationLanguages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMemberLocalizations_LocalizationLanguages_LanguageId",
                table: "TeamMemberLocalizations",
                column: "LanguageId",
                principalTable: "LocalizationLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
