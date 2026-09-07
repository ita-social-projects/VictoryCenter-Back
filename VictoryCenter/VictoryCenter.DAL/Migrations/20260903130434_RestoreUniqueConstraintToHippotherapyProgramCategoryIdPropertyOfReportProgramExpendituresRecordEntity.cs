using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RestoreUniqueConstraintToHippotherapyProgramCategoryIdPropertyOfReportProgramExpendituresRecordEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportProgramExpendituresRecords_HippotherapyProgramCategoryId_ReportingYear",
                table: "ReportProgramExpendituresRecords");

            // WARNING: irreversible data change. To allow one record per program category, any
            // category with several records keeps only its most recent row (latest CreatedAt, then
            // highest Id) and the rest are deleted after being copied to
            // dbo.RemovedReportProgramExpendituresRecordDuplicates. The block is a no-op when there
            // are no duplicates.
            migrationBuilder.Sql("""
                DECLARE @DuplicateIds TABLE (Id BIGINT PRIMARY KEY);

                INSERT INTO @DuplicateIds (Id)
                SELECT Id
                FROM (
                    SELECT Id,
                           ROW_NUMBER() OVER (
                               PARTITION BY HippotherapyProgramCategoryId
                               ORDER BY CreatedAt DESC, Id DESC) AS RowNumber
                    FROM dbo.ReportProgramExpendituresRecords
                ) AS ranked
                WHERE ranked.RowNumber > 1;

                IF EXISTS (SELECT 1 FROM @DuplicateIds)
                BEGIN
                    DECLARE @RemovedCount INT = (SELECT COUNT(*) FROM @DuplicateIds);
                    DECLARE @RemovedIds NVARCHAR(MAX) =
                        (SELECT STRING_AGG(CONVERT(NVARCHAR(20), Id), ', ') FROM @DuplicateIds);

                    DROP TABLE IF EXISTS dbo.RemovedReportProgramExpendituresRecordDuplicates;

                    SELECT r.*, SYSUTCDATETIME() AS RemovedAtUtc
                    INTO dbo.RemovedReportProgramExpendituresRecordDuplicates
                    FROM dbo.ReportProgramExpendituresRecords AS r
                    WHERE r.Id IN (SELECT Id FROM @DuplicateIds);

                    DELETE FROM dbo.ReportProgramExpendituresRecords
                    WHERE Id IN (SELECT Id FROM @DuplicateIds);

                    PRINT CONCAT(
                        'RestoreUniqueConstraintToHippotherapyProgramCategoryId migration: removed ',
                        @RemovedCount,
                        ' duplicate ReportProgramExpendituresRecords row(s); backup saved to ',
                        'dbo.RemovedReportProgramExpendituresRecordDuplicates. Removed Ids: ',
                        @RemovedIds);
                END;

                IF EXISTS (
                    SELECT 1
                    FROM dbo.ReportProgramExpendituresRecords
                    GROUP BY HippotherapyProgramCategoryId
                    HAVING COUNT(*) > 1)
                BEGIN
                    THROW 50001,
                        'Duplicate ReportProgramExpendituresRecords remain after cleanup; migration aborted.',
                        1;
                END;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_ReportProgramExpendituresRecords_HippotherapyProgramCategoryId",
                table: "ReportProgramExpendituresRecords",
                column: "HippotherapyProgramCategoryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportProgramExpendituresRecords_HippotherapyProgramCategoryId",
                table: "ReportProgramExpendituresRecords");

            migrationBuilder.CreateIndex(
                name: "IX_ReportProgramExpendituresRecords_HippotherapyProgramCategoryId_ReportingYear",
                table: "ReportProgramExpendituresRecords",
                columns: new[] { "HippotherapyProgramCategoryId", "ReportingYear" },
                unique: true);
        }
    }
}
