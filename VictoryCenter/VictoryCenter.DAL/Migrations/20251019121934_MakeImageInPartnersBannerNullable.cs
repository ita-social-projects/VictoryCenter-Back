using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeImageInPartnersBannerNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnersPageBanners_Images_ImageId",
                table: "PartnersPageBanners");

            migrationBuilder.DropIndex(
                name: "IX_PartnersPageBanners_ImageId",
                table: "PartnersPageBanners");

            migrationBuilder.AlterColumn<long>(
                name: "ImageId",
                table: "PartnersPageBanners",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_PartnersPageBanners_ImageId",
                table: "PartnersPageBanners",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnersPageBanners_Images_ImageId",
                table: "PartnersPageBanners",
                column: "ImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnersPageBanners_Images_ImageId",
                table: "PartnersPageBanners");

            migrationBuilder.DropIndex(
                name: "IX_PartnersPageBanners_ImageId",
                table: "PartnersPageBanners");

            migrationBuilder.AlterColumn<long>(
                name: "ImageId",
                table: "PartnersPageBanners",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartnersPageBanners_ImageId",
                table: "PartnersPageBanners",
                column: "ImageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnersPageBanners_Images_ImageId",
                table: "PartnersPageBanners",
                column: "ImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
