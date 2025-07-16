using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddImageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "TeamMembers");

            migrationBuilder.EnsureSchema(
                name: "media");

            migrationBuilder.AddColumn<long>(
                name: "ImageId",
                table: "TeamMembers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "media",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlobName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ImageId",
                table: "TeamMembers",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Images_ImageId",
                table: "TeamMembers",
                column: "ImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Images_ImageId",
                table: "TeamMembers");

            migrationBuilder.DropTable(
                name: "Images",
                schema: "media");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_ImageId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "TeamMembers");

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "TeamMembers",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
