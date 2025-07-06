using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addImageLogic2 : Migration
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

            migrationBuilder.AddColumn<long>(
                name: "ImageId1",
                table: "TeamMembers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "images",
                schema: "media",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MimeType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BlobName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ImageId",
                table: "TeamMembers",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ImageId1",
                table: "TeamMembers",
                column: "ImageId1",
                unique: true,
                filter: "[ImageId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_images_ImageId",
                table: "TeamMembers",
                column: "ImageId",
                principalSchema: "media",
                principalTable: "images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_images_ImageId1",
                table: "TeamMembers",
                column: "ImageId1",
                principalSchema: "media",
                principalTable: "images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_images_ImageId",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_images_ImageId1",
                table: "TeamMembers");

            migrationBuilder.DropTable(
                name: "images",
                schema: "media");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_ImageId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_ImageId1",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "ImageId1",
                table: "TeamMembers");

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "TeamMembers",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
