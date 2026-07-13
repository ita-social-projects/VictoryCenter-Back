using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageIdToPdfReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PdfReports_Priority",
                schema: "media",
                table: "PdfReports");

            migrationBuilder.Sql("""
                INSERT INTO LocalizationLanguages (Code, Name)
                SELECT 'uk', N'Українська'
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM LocalizationLanguages
                    WHERE Code = 'uk'
                );
                """);

            migrationBuilder.AddColumn<long>(
                name: "LanguageId",
                schema: "media",
                table: "PdfReports",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE pr
                SET LanguageId = ll.Id
                FROM media.PdfReports pr
                CROSS JOIN LocalizationLanguages ll
                WHERE ll.Code = 'uk'
                  AND pr.LanguageId IS NULL;
             """);

            migrationBuilder.AlterColumn<long>(
                name: "LanguageId",
                schema: "media",
                table: "PdfReports",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PdfReports_LanguageId_Priority",
                schema: "media",
                table: "PdfReports",
                columns: new[] { "LanguageId", "Priority" },
                unique: true);

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
                name: "IX_PdfReports_LanguageId_Priority",
                schema: "media",
                table: "PdfReports");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                schema: "media",
                table: "PdfReports");

            migrationBuilder.CreateIndex(
                name: "IX_PdfReports_Priority",
                schema: "media",
                table: "PdfReports",
                column: "Priority",
                unique: true);
        }
    }
}
