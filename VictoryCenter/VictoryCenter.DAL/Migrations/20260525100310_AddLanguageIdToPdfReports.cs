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
            // CreatedAt is required and has no database default, so the fallback language must provide it explicitly.
            migrationBuilder.Sql("""
                INSERT INTO dbo.LocalizationLanguages (Code, Name, CreatedAt)
                SELECT 'uk', N'Українська', TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00')
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM dbo.LocalizationLanguages
                    WHERE Code = 'uk'
                );
                """);

            // Clean databases do not have this column, while some environments
            // already received it from the removed migration.
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'media.PdfReports', N'LanguageId') IS NULL
                BEGIN
                    ALTER TABLE media.PdfReports ADD LanguageId bigint NULL;
                END;
                """);

            // Remove artifacts created by the removed migration before applying the current model constraints.
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = N'FK_PdfReports_LocalizationLanguages_LanguageId'
                      AND parent_object_id = OBJECT_ID(N'media.PdfReports')
                )
                BEGIN
                    ALTER TABLE media.PdfReports
                    DROP CONSTRAINT FK_PdfReports_LocalizationLanguages_LanguageId;
                END;

                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_PdfReports_LanguageId'
                      AND object_id = OBJECT_ID(N'media.PdfReports')
                )
                BEGIN
                    DROP INDEX IX_PdfReports_LanguageId ON media.PdfReports;
                END;
                """);

            // The current model defines priority as unique within a language, rather than globally across all reports.
            migrationBuilder.DropIndex(
                name: "IX_PdfReports_Priority",
                schema: "media",
                table: "PdfReports");

            // Existing rows must be backfilled before the initially nullable column can be made required.
            // Code has a unique index, so the cross join resolves to exactly one fallback language.
            migrationBuilder.Sql("""
                UPDATE pr
                SET LanguageId = ll.Id
                FROM media.PdfReports pr
                CROSS JOIN dbo.LocalizationLanguages ll
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

            // Allow the same priority in different languages while preventing duplicates within one language.
            migrationBuilder.CreateIndex(
                name: "IX_PdfReports_LanguageId_Priority",
                schema: "media",
                table: "PdfReports",
                columns: new[] { "LanguageId", "Priority" },
                unique: true);

            // Prevent deletion of a language while PDF reports still reference it.
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

            // Restore the schema owned by the removed migration when its history row remains.
            // Otherwise, remove the column introduced by this migration on a clean database.
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM entity_framework.__EFMigrationsHistory
                    WHERE MigrationId = N'20260609211541_AddLanguageIdToPdfReport'
                )
                BEGIN
                    ALTER TABLE media.PdfReports
                    ALTER COLUMN LanguageId bigint NULL;

                    CREATE INDEX IX_PdfReports_LanguageId
                    ON media.PdfReports (LanguageId);

                    ALTER TABLE media.PdfReports
                    ADD CONSTRAINT FK_PdfReports_LocalizationLanguages_LanguageId
                    FOREIGN KEY (LanguageId)
                    REFERENCES dbo.LocalizationLanguages (Id)
                    ON DELETE NO ACTION;
                END;
                ELSE
                BEGIN
                    ALTER TABLE media.PdfReports DROP COLUMN LanguageId;
                END;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_PdfReports_Priority",
                schema: "media",
                table: "PdfReports",
                column: "Priority",
                unique: true);
        }
    }
}
