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

            // A program category may now hold only a single record regardless of reporting year.
            // Collapse any pre-existing cross-year duplicates down to the most recently created
            // record per category so the unique index below can be created.
            migrationBuilder.Sql("""
                DELETE r
                FROM dbo.ReportProgramExpendituresRecords AS r
                WHERE r.Id < (
                    SELECT MAX(r2.Id)
                    FROM dbo.ReportProgramExpendituresRecords AS r2
                    WHERE r2.HippotherapyProgramCategoryId = r.HippotherapyProgramCategoryId
                );
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
