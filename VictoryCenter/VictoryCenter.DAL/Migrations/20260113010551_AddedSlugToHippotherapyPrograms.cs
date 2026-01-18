using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedSlugToHippotherapyPrograms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "HippotherapyPrograms",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyPrograms_Slug",
                table: "HippotherapyPrograms",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HippotherapyPrograms_Slug",
                table: "HippotherapyPrograms");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "HippotherapyPrograms");
        }
    }
}
