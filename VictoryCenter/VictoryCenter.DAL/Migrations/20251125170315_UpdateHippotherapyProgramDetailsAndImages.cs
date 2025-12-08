using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHippotherapyProgramDetailsAndImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HippotherapyPrograms_Images_ImageId",
                table: "HippotherapyPrograms");

            migrationBuilder.DropIndex(
                name: "IX_HippotherapyPrograms_ImageId",
                table: "HippotherapyPrograms");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "HippotherapyPrograms",
                newName: "PreviewImageId");

            migrationBuilder.AddColumn<long>(
                name: "BackgroundImageId",
                table: "HippotherapyPrograms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "HippotherapyPrograms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetingsCount",
                table: "HippotherapyPrograms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantsCount",
                table: "HippotherapyPrograms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyPrograms_BackgroundImageId",
                table: "HippotherapyPrograms",
                column: "BackgroundImageId",
                unique: true,
                filter: "[BackgroundImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyPrograms_PreviewImageId",
                table: "HippotherapyPrograms",
                column: "PreviewImageId",
                unique: true,
                filter: "[PreviewImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_HippotherapyPrograms_Images_BackgroundImageId",
                table: "HippotherapyPrograms",
                column: "BackgroundImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HippotherapyPrograms_Images_PreviewImageId",
                table: "HippotherapyPrograms",
                column: "PreviewImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HippotherapyPrograms_Images_BackgroundImageId",
                table: "HippotherapyPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_HippotherapyPrograms_Images_PreviewImageId",
                table: "HippotherapyPrograms");

            migrationBuilder.DropIndex(
                name: "IX_HippotherapyPrograms_BackgroundImageId",
                table: "HippotherapyPrograms");

            migrationBuilder.DropIndex(
                name: "IX_HippotherapyPrograms_PreviewImageId",
                table: "HippotherapyPrograms");

            migrationBuilder.DropColumn(
                name: "BackgroundImageId",
                table: "HippotherapyPrograms");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "HippotherapyPrograms");

            migrationBuilder.DropColumn(
                name: "MeetingsCount",
                table: "HippotherapyPrograms");

            migrationBuilder.DropColumn(
                name: "ParticipantsCount",
                table: "HippotherapyPrograms");

            migrationBuilder.RenameColumn(
                name: "PreviewImageId",
                table: "HippotherapyPrograms",
                newName: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyPrograms_ImageId",
                table: "HippotherapyPrograms",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_HippotherapyPrograms_Images_ImageId",
                table: "HippotherapyPrograms",
                column: "ImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
