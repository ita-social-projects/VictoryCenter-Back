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
